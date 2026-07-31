"use client";

import Surface from "@/src/shared/components/surfaces/Surface"
import MyInfoForm, { MyInfoFormData, MyInfoFormError } from "./MyInfoForm"
import { User, UserPermission } from "@/src/shared/types/domain/aggregates"
import { ReadDto } from "@/src/shared/types/dtos/types"
import { useEffect, useState } from "react"
import Button from "@/src/shared/components/buttons/Button";
import { addImages, updateProfile } from "../actions/action";
import * as z from "zod";
import { Gender } from "@/src/shared/types/domain/value-objects";
import LargeModal from "@/src/shared/components/modal/LargeModal";
import Profile from "@/src/shared/components/user/Profile";
import { calculateAge } from "@/src/shared/time";
import AlertDialog from "@/src/shared/components/infos/AlertDialog";
import { useRouter } from "nextjs-toploader/app";
import { Guid, NullGuid } from "@/src/shared/cache";
import { useApplicationStore } from "@/src/shared/stores/provider";
import { cropImage } from "@/src/shared/image";
import { getBase64EncodedData } from "@/src/shared/encoding";
import ErrorMessage from "@/src/shared/components/inputs/ErrorMessage";

export type MeProps = {
    className?: string;
}
export default function Me({ className }: MeProps) {

    const router = useRouter();

    const user = useApplicationStore(s => s.user);
    const setUser = useApplicationStore(s => s.setUser);


    const [showPreview, setShowPreview] = useState(false);
    function getBasicFormDataFromCurrentUser(user: ReadDto<User, UserPermission> | undefined) {
        const images = [...(user?.data.images ?? [])];
        const basicFormData = {
            images: images
                .map(image => ({
                    ...image.imageDetails,
                    id: image.id,
                }))
                .sort((a, b) => a.order - b.order) ?? [],
            alias: user?.data?.alias!,
            birthDate: user?.data?.birthDate!,
            gender: user?.data?.gender,
            location: user?.data?.location,
            bio: user?.data?.bio
        };

        return basicFormData;
    }

    const [serverSideBasicFormData, setServerSideBasicFormData] = useState<MyInfoFormData>(getBasicFormDataFromCurrentUser(user));
    const [clientSideBasicFormData, setClientSideBasicFormData] = useState<MyInfoFormData>(getBasicFormDataFromCurrentUser(user));


    useEffect(() => {

        const updatedBasicFormData = getBasicFormDataFromCurrentUser(user)
        setServerSideBasicFormData(updatedBasicFormData);
        // Warning! If the user adds images after issuing a save, we'll reach this point and we'll clear his added images.
        // This is not something we can solve. There is no way to correlate a locally added image with a server-side image, because the local
        // image's id is undefined.
        setClientSideBasicFormData(prev => ({
            ...prev,
            images: updatedBasicFormData.images
        }));

    }, [user]);

    const [basicFormError, setBasicFormError] = useState<MyInfoFormError>({});

    const validationScheme = z.object({
        images: z.array(z.object({
            id: z.string().optional(),
            url: z.url(),
            mimeType: z.string(),
            order: z.number(),
        })).min(1, { error: "you must add at least 1 image" }).max(10, { error: "you can add at most 10 images" }),
        alias: z.string().min(1, {error: "alias is required"}),
        birthDate: z.string().min(1, {error: "birthDate is required"}),
        gender: z.enum(Gender, { error: "gender is required" }),
        location: z.object({
            latitude: z.number(),
            longitude: z.number()
        }).optional(),
        bio: z.string().optional()
    });

    function onCancel() {
        setBasicFormError({});
        setClientSideBasicFormData(serverSideBasicFormData);
    }

    function onPreview() {
        setShowPreview(true);
    }

    async function onSave() {

        const validationResult = validationScheme.safeParse(clientSideBasicFormData);
        if (!validationResult.success) {
            const error = z.treeifyError(validationResult.error);
            setBasicFormError({
                alias: error.properties?.alias?.errors?.[0],
                birthDate: error.properties?.birthDate?.errors?.[0],
                gender: error.properties?.gender?.errors?.[0],
                images: error.properties?.images?.errors?.[0]
            });
            return;
        }

        setBasicFormError({});

        const imageAndIndex = clientSideBasicFormData.images?.map((image, index) => ({ image, index })) ?? [];
        const imagesToKeep = imageAndIndex.filter(({ image }) => image.id !== undefined).map(({ image, index }) => ({ id: image.id!, order: index }));
        const imagesToAdd = imageAndIndex.filter(({ image }) => image.id === undefined);

        let addedImages: { id: Guid, order: number }[] = [];

        if (imagesToAdd.length > 0) {
            const payload = await Promise.all(imagesToAdd.map(({ image, index }) => ({
                ...image,
                order: index
            })).map(async image => ({
                image,
                base64Data: await getBase64EncodedData(image.url!)
            })));

            const addImagesResult = await addImages(payload);
            if (!addImagesResult.success) {
                throw new Error("not implemented");
            }

            addedImages = addImagesResult.value?.images.map(i => ({ id: i.id, order: i.imageDetails.order })) ?? [];
        }

        const updateProfileResponse = await updateProfile
            (
                user?.data?.id!,
                clientSideBasicFormData.alias!,
                clientSideBasicFormData.birthDate!,
                clientSideBasicFormData.gender,
                clientSideBasicFormData.location,
                imagesToKeep.concat(addedImages),
                clientSideBasicFormData.bio
            );

        if (!updateProfileResponse.success) {
            throw new Error("not implemented");
        }

        const responseReadDto = updateProfileResponse.value!;
        setUser(_ => responseReadDto);
        router.refresh();
    }

    const classNames = [
        className
    ].filter(Boolean).join(" ")
    return (

        <Surface className={`${classNames} flex flex-col gap-2`} padding="none">
            <MyInfoForm className="grow-1 overflow-y-scroll scrollbar-none" error={basicFormError} data={clientSideBasicFormData} onChange={(data) => {

                setClientSideBasicFormData(prev => ({
                    ...prev,
                    images: data.images?.sort((a, b) => a.order - b.order),
                    alias: data.alias,
                    birthDate: data.birthDate,
                    gender: data.gender,
                    location: data.location,
                    bio: data.bio
                }));

            }} />

            {
                basicFormError.images && 
                <Surface variant="secondary" className="flex flex-col items-center justify-center">
                    <ErrorMessage message={basicFormError.images} />
                </Surface>
            }
            <div className="flex flex-row gap-3 justify-end">
                {(serverSideBasicFormData.location === undefined || clientSideBasicFormData.location !== undefined) && <Button onClick={onSave} intent="save">save</Button>}
                {
                    (serverSideBasicFormData.location !== undefined) && (clientSideBasicFormData.location === undefined) &&
                    <AlertDialog
                        title="are you sure?"
                        description='clearing your location will disable the distance filter on all of your search profiles'
                        onCancel={() => {
                            setClientSideBasicFormData(prev => ({
                                ...prev,
                                location: serverSideBasicFormData.location
                            }))
                        }}
                        onConfirm={onSave}
                    >
                        <Button intent="save">save</Button>
                    </AlertDialog>
                }
                <Button onClick={onPreview} intent="preview">preview</Button>
                <Button onClick={onCancel} intent="cancel">cancel</Button>
            </div>
            {
                showPreview && <LargeModal header="preview" className="surface-secondary" hidden={false} closed={() => setShowPreview(false)}>
                    <Profile
                        candidate={{
                            id: user?.data?.id!,
                            bio: clientSideBasicFormData.bio,
                            alias: clientSideBasicFormData.alias,
                            birthDate: clientSideBasicFormData.birthDate,
                            gender: clientSideBasicFormData.gender,
                            datingPreferences: user?.data?.datingPreferences,
                            location: clientSideBasicFormData.location,
                            distanceKmToQueryingUser: user?.data?.distanceKmToQueryingUser || 0,
                            age: clientSideBasicFormData.birthDate ? calculateAge(clientSideBasicFormData.birthDate) : undefined,
                            createdAt: user?.data?.createdAt,
                            updatedAt: user?.data?.updatedAt,
                            email: user?.data?.email,
                            images: clientSideBasicFormData.images
                                ?.map((image, index) => ({ image, index }))
                                .map(({ image, index }) => {
                                    return {
                                        id: image.id ?? NullGuid,
                                        ownerUserId: NullGuid,
                                        createdAt: "",
                                        updatedAt: new Date().toISOString(),
                                        imageDetails: {
                                            url: image.url,
                                            mimeType: image.mimeType,
                                            order: index,
                                            name: "",
                                            largeUrl: image.largeUrl,
                                            mediumUrl: image.mediumUrl,
                                            smallUrl: image.smallUrl
                                        },
                                        markedForDeletionAt: null,
                                        deletedAt: null,
                                    }
                                }) ?? []
                        }}
                    />
                </LargeModal>
            }
        </Surface>

    )

}