"use client";

import Surface from "@/src/shared/components/surfaces/Surface"
import MyInfoForm, { MyInfoFormData, MyInfoFormError } from "./MyInfoForm"
import { User, UserPermission } from "@/src/shared/types/domain/aggregates"
import { ReadDto } from "@/src/shared/types/dtos/types"
import { useEffect, useState } from "react"
import Button from "@/src/shared/components/buttons/Button";
import { updateProfile } from "../actions/action";
import * as z from "zod";
import { Gender, ImageStatus } from "@/src/shared/types/domain/value-objects";
import LargeModal from "@/src/shared/components/modal/LargeModal";
import Profile from "@/src/shared/components/user/Profile";
import { calculateAge } from "@/src/shared/time";
import AlertDialog from "@/src/shared/components/infos/AlertDialog";
import { useRouter } from "nextjs-toploader/app";
import { Mutate } from "@/src/shared/apis/api";
import { FailureReason } from "@/src/shared/apis/type";
import { AddImageResult } from "../actions/type";
import { NullGuid } from "@/src/shared/cache";
import { useApplicationStore } from "@/src/shared/stores/provider";

export type MeProps = {
    className?: string;
}
export default function Me({ className }: MeProps) {

    const router = useRouter();

    const user = useApplicationStore(s => s.user);
    const setUser = useApplicationStore(s => s.setUser);


    const [showPreview, setShowPreview] = useState(false);
    function getBasicFormDataFromCurrentUser(user: ReadDto<User, UserPermission> | undefined) {
        const images = [...(user?.data.pendingImages ?? []), ...(user?.data.acceptedImages ?? [])];
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
        })).optional(),
        alias: z.string().min(1, "alias is required"),
        birthDate: z.string().min(1, "birthDate is required"),
        gender: z.enum(Gender).optional(),
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

    async function addImage(blob: Blob, mimeType: string, order: number, crop: { x: number, y: number, width: number, height: number }): Promise<AddImageResult> {

        const body = JSON.stringify({ mimeType, order, crop })
        const getSasResponse = await Mutate<AddImageResult, FailureReason>(
            `${process.env.NEXT_PUBLIC_API_URL}/api/user/upload-image`,
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body
            },
        );

        if (!getSasResponse.success || !getSasResponse.value?.uploadUrl) {
            throw new Error("not implemented");
        }

        const uploadUrl = getSasResponse.value;
        const response = await fetch(uploadUrl.uploadUrl!, {
            method: "PUT",
            headers: {
                "x-ms-blob-type": "BlockBlob",
                "Content-Type": mimeType
            },
            body: blob
        })

        if (!response.ok) {
            throw new Error("not implemented");
        }

        return getSasResponse.value;

    }

    async function onSave() {

        const validationResult = validationScheme.safeParse(clientSideBasicFormData);
        if (!validationResult.success) {
            const error = z.treeifyError(validationResult.error);
            setBasicFormError({
                alias: error.properties?.alias?.errors?.[0],
                birthDate: error.properties?.birthDate?.errors?.[0]
            });
            return;
        }

        setBasicFormError({});

        const imageAndIndex = clientSideBasicFormData.images?.map((image, index) => ({ image, index })) ?? [];
        const imagesToKeep = imageAndIndex.filter(({ image }) => image.id !== undefined).map(({ image, index }) => ({ id: image.id!, order: index }));
        const imagesToAdd = imageAndIndex.filter(({ image }) => image.id === undefined);

        const addImageResults = await Promise.all(imagesToAdd.filter(i => i.image.url !== undefined).map(async i => {
            const blob = await (await fetch(i.image.url!)).blob();
            return await addImage(blob, i.image.mimeType, i.image.order, i.image.crop!)
        }));

        const updateProfileResponse = await updateProfile
        (
            user?.data?.id!,
            clientSideBasicFormData.alias!,
            clientSideBasicFormData.birthDate!,
            clientSideBasicFormData.gender,
            clientSideBasicFormData.location,
            imagesToKeep.concat(addImageResults.map(i => ({ id: i.image.id, order: i.image.imageDetails.order }))),
            clientSideBasicFormData.bio
        );

        if (!updateProfileResponse.success) {
            throw new Error("not implemented");
        }

        const responseReadDto = updateProfileResponse.value!;
        setUser(responseReadDto);
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
                    images: data.images?.sort((a, b) => a.order - b.order) ?? [],
                    alias: data.alias,
                    birthDate: data.birthDate,
                    gender: data.gender,
                    location: data.location,
                    bio: data.bio
                }));
            }} />
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
                showPreview && <LargeModal className="surface-secondary" hidden={false} closed={() => setShowPreview(false)} header={undefined}>
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
                            acceptedImages: clientSideBasicFormData.images
                                ?.map((image, index) => ({ image, index }))
                                .filter(({ image }) => image.status === ImageStatus.Accepted || image.status === undefined)
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
                                            status: image.status ?? ImageStatus.Pending,
                                            largeUrl: image.largeUrl,
                                            mediumUrl: image.mediumUrl,
                                            smallUrl: image.smallUrl
                                        },
                                        markedForDeletionAt: null,
                                        deletedAt: null,
                                    }
                                }) ?? [],
                            pendingImages: clientSideBasicFormData.images
                                ?.map((image, index) => ({ image, index }))
                                .filter(({ image }) => image.status === ImageStatus.Pending)
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
                                            status: image.status ?? ImageStatus.Pending
                                        },
                                        markedForDeletionAt: null,
                                        deletedAt: null,
                                    }
                                }) ?? [],
                        }}
                    />
                </LargeModal>
            }
        </Surface>

    )

}