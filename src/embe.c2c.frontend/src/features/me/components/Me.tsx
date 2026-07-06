"use client";

import Surface from "@/src/shared/components/surfaces/Surface"
import MyBasicInfoForm, { MyBasicInfoFormData } from "./MyBasicInfoForm"
import { User, UserPermission } from "@/src/shared/types/domain/aggregates"
import { ReadDto } from "@/src/shared/types/dtos/types"
import { useState } from "react"
import Button from "@/src/shared/components/buttons/Button";
import { updateProfile } from "../actions/action";
import * as z from "zod";
import { Gender } from "@/src/shared/types/domain/value-objects";

export type MeProps = {
    className?: string,
    user: ReadDto<User, UserPermission>
}

export default function Me({ className, user }: MeProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ")

    const initialBasicFormData = {
        images: user.data?.images
            ?.map(image => ({ id: image.id, url: image.imageDetails.url, mimeType: image.imageDetails.mimeType, order: image.imageDetails.order }))
            .sort((a, b) => a.order - b.order) ?? [],
        alias: user.data?.alias!,
        birthDate: user.data?.birthDate!,
        gender: user.data?.gender,
        location: user.data?.location
    };

    const [serverSideBasicFormData, setServerSideBasicFormData] = useState<MyBasicInfoFormData>(initialBasicFormData);
    const [clientSideBasicFormData, setClientSideBasicFormData] = useState<MyBasicInfoFormData>(initialBasicFormData);

    const validationScheme = z.object({
        images: z.array(z.object({
            id: z.string().optional(),
            url: z.url(),
            mimeType: z.string(),
            order: z.number()
        })).optional(),
        alias: z.string().min(1),
        birthDate: z.string().min(1),
        gender: z.enum(Gender).optional(),
        location: z.object({
            latitude: z.number(),
            longitude: z.number()
        }).optional()
    });

    function onCancel() {
        setClientSideBasicFormData(serverSideBasicFormData);
    }

    async function onSave() {

        const validationResult = validationScheme.safeParse(clientSideBasicFormData);
        if (!validationResult.success) {
            throw new Error("not implemented");
        }

        const imageAndIndex = clientSideBasicFormData.images?.map((image, index) => ({ image, index })) ?? [];
        const imagesToKeep = imageAndIndex.filter(({ image }) => image.id !== undefined).map(({ image, index }) => ({ id: image.id!, order: index }));
        const imagesToAdd = imageAndIndex.filter(({ image }) => image.id === undefined);

        const response = await updateProfile(
            user.data?.id!,
            clientSideBasicFormData.alias!,
            clientSideBasicFormData.birthDate!,
            clientSideBasicFormData.gender,
            clientSideBasicFormData.location,
            imagesToKeep,
            imagesToAdd.map(({ image, index }) => ({ url: image.url, mimeType: image.mimeType, order: index }))
        );

        if (!response.success) {
            throw new Error("not implemented");
        }

        const responseReadDto = response.value!;
        const newServerSideBasicFormData = {
            images: responseReadDto.data.images
                ?.map(image => ({ id: image.id, url: image.imageDetails.url, mimeType: image.imageDetails.mimeType, order: image.imageDetails.order }))
                .sort((a, b) => a.order - b.order) ?? [],
            alias: responseReadDto.data.alias!,
            birthDate: responseReadDto.data.birthDate!,
            gender: responseReadDto.data.gender,
            location: responseReadDto.data.location
        };

        setServerSideBasicFormData(newServerSideBasicFormData);
        setClientSideBasicFormData(newServerSideBasicFormData);
    }

    return (

        // Information
        // ------------
        // Images
        // Alias
        // BirthDate
        // Gender
        // Location

        <Surface className={`${classNames} flex flex-col gap-2`} padding="none">
            <MyBasicInfoForm className="grow-1 overflow-y-scroll" data={clientSideBasicFormData} onChange={(data) => {
                setClientSideBasicFormData(prev => ({
                    ...prev,
                    images: data.images?.sort((a, b) => a.order - b.order) ?? [],
                    alias: data.alias,
                    birthDate: data.birthDate,
                    gender: data.gender,
                    location: data.location
                }));
            }} />
            <div className="flex flex-row gap-3 justify-end">
                <Button onClick={onSave}>save</Button>
                <Button variant="secondary" onClick={onCancel}>cancel</Button>
            </div>
        </Surface>

    )

}