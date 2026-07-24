"use client";

import Surface from "@/src/shared/components/surfaces/Surface"
import { useState } from "react";
import LargeModal from "@/src/shared/components/modal/LargeModal";
import ImageGalleryInput from "@/src/shared/components/inputs/image/gallery/ImageGalleryInput";
import { Gender, ImageStatus } from "@/src/shared/types/domain/value-objects";
import { Location } from "@/src/shared/types/domain/value-objects";
import BasicProfileForm from "../../auth/components/BasicProfileForm";
import { getValidBirthdateRange } from "@/src/shared/time";
import TextAreaInput from "@/src/shared/components/inputs/text-area-input/TextAreaInput";
import Image from "@/src/shared/components/images/Image";
import { Guid } from "@/src/shared/cache";
import Button from "@/src/shared/components/buttons/Button";
import { Plus } from "@deemlol/next-icons";

export type ImageData = {
    id?: Guid,
    url?: string,
    largeUrl?: string,
    mediumUrl?: string,
    smallUrl?: string,
    mimeType: string,
    order: number;
    status?: ImageStatus;
    crop?: {
        x: number,
        y: number,
        width: number,
        height: number
    }
}
type MyImagesFormProps = {
    initialImages: ImageData[]
    onChange?: (images: ImageData[]) => void
    className?: string
}
function MyImagesForm({ initialImages, onChange, className }: MyImagesFormProps) {

    const [modalOpen, setModalOpen] = useState(false);
    const isEmpty = initialImages.length === 0;
    const profilePicture = !isEmpty ? initialImages.sort((a, b) => a.order - b.order)[0] : undefined;
    const profilePictureUrl = profilePicture?.smallUrl ?? profilePicture?.mediumUrl ?? profilePicture?.largeUrl ?? profilePicture?.url;
    const classNames = [
        className
    ].filter(Boolean).join(" ")

    return (
        <div className={`max-w-max ${classNames}`}>
            <Button onClick={() => setModalOpen(prev => !prev)} className="bg-transparent text-(length:--fs-1)">
                {
                    !isEmpty && profilePictureUrl && <Image
                        className={`rounded-full w-[150px] h-[150px] object-cover shadow-(color:--border-color) shadow-md`}
                        src={profilePictureUrl}
                        alt="User Image"
                        width={150}
                        height={150}
                        unoptimized={process.env.NODE_ENV === "development"}
                    />
                }
                {
                    (isEmpty || !profilePictureUrl) &&
                    <div className={`rounded-full w-[150px] h-[150px] flex flex-col items-center justify-center bg-gray-300`}>
                        <Plus className="w-(--primary-fs) h-(--primary-fs) text-(--primary-fc)" />
                    </div>
                }
            </Button>

            <LargeModal closed={() => setModalOpen(false)} hidden={!modalOpen} header="images">
                <ImageGalleryInput<ImageData>
                    data={{ images: initialImages }}
                    onChange={
                        (newImages) => {
                            const result = newImages.map((image, index) => ({
                                ...image,
                                id: (image as ImageData)?.id,
                                order: index
                            }));
                            onChange?.(result);
                        }
                    }
                />
            </LargeModal>
        </div>
    )
}

export type MyInfoFormData = {
    alias?: string;
    birthDate?: string;
    images?: ImageData[];
    gender?: Gender;
    location?: Location;
    bio?: string;
}

export type MyInfoFormError = { [P in keyof MyInfoFormData]?: string }

export type MyInfoFormProps = {
    className?: string
    data: MyInfoFormData,
    error?: MyInfoFormError,
    onChange: (data: MyInfoFormData) => void
}
export default function MyInfoForm({ className, data, error, onChange }: MyInfoFormProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ")

    return (

        <div className={`w-full flex flex-col gap-2 ${classNames}`}>

            <Surface className={`w-full flex flex-col gap-2`} padding="md" variant="secondary">

                <MyImagesForm
                    className="w-full shrink-0 mx-auto"
                    initialImages={data.images ?? []}
                    onChange={(images) => onChange({ ...data, images })}
                />

                <BasicProfileForm
                    error={{
                        alias: error?.alias,
                        birthDate: error?.birthDate,
                        location: error?.location,
                        gender: error?.gender
                    }}
                    config={{
                        alias: true,
                        birthDate: true,
                        gender: true,
                        location: true
                    }}
                    data={{
                        birthDateRange: getValidBirthdateRange(18, 120),
                        birthDate: data.birthDate,
                        alias: data.alias,
                        gender: data.gender,
                        location: data.location,
                    }}
                    onChange={basicProfileFormData => {
                        onChange(({
                            ...data,
                            birthDate: basicProfileFormData.birthDate,
                            alias: basicProfileFormData.alias,
                            gender: basicProfileFormData.gender,
                            location: basicProfileFormData.location
                        }))
                    }
                    }

                />
            </Surface>

            <Surface variant="secondary" padding="md">
                <TextAreaInput
                    size="lg"
                    placeholder="tell the world about yourself..."
                    label="bio"
                    value={data.bio}
                    onBlur={(value) => onChange({ ...data, bio: value })}
                    errorMessage={error?.bio}
                />
            </Surface>

        </div>

    )

}