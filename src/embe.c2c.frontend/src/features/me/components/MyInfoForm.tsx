"use client";

import Surface from "@/src/shared/components/surfaces/Surface"
import { useState } from "react";
import LargeModal from "@/src/shared/components/modal/LargeModal";
import ImageGalleryInput from "@/src/shared/components/inputs/image/gallery/ImageGalleryInput";
import { Gender } from "@/src/shared/types/domain/value-objects";
import { Location } from "@/src/shared/types/domain/value-objects";
import BasicProfileForm from "../../auth/components/BasicProfileForm";
import { getValidBirthdateRange } from "@/src/shared/time";
import TextAreaInput from "@/src/shared/components/inputs/text-area-input/TextAreaInput";
import Image from "@/src/shared/components/images/Image";

export type ImageData = {
    id?: string,
    url?: string,
    mimeType: string,
    order: number
}
type MyImagesFormProps = {
    initialImages: ImageData[]
    onChange?: (images: ImageData[]) => void
    className?: string
}
function MyImagesForm({ initialImages, onChange, className }: MyImagesFormProps) {

    const [modalOpen, setModalOpen] = useState(false);
    const isEmpty = initialImages.length === 0;
    const classNames = [
        className
    ].filter(Boolean).join(" ")

    return (
        <div className={`max-w-max ${classNames}`}>
            <button onClick={() => setModalOpen(prev => !prev)} className="bg-transparent text-(length:--fs-1)">
                {
                    !isEmpty && <Image
                        className={`rounded-full w-[150px] h-[150px] object-cover shadow-(color:--border-color) shadow-md`}
                        src={initialImages.find(image => image.order === 0)?.url ?? ""}
                        alt="User Image"
                        width={150}
                        height={150}
                        unoptimized={process.env.NODE_ENV === "development"}
                    />
                }
                {
                    isEmpty &&
                    <div className={`rounded-full w-[150px] h-[150px] flex flex-col items-center justify-center bg-gray-300`}>
                    </div>
                }
            </button>

            <LargeModal closed={() => setModalOpen(false)} hidden={!modalOpen} header="images">
                <ImageGalleryInput<ImageData>
                    data={{ images: initialImages }}
                    onChange={
                        (newImages) => {
                            onChange?.(newImages.map((image, index) => ({ id: (image as ImageData)?.id, url: image.url, mimeType: image.mimeType, order: index })));
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
                    initialValue={data.bio}
                    onBlur={(value) => onChange({ ...data, bio: value })}
                    errorMessage={error?.bio}
                />
            </Surface>

        </div>

    )

}