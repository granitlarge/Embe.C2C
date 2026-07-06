"use client";

import Surface from "@/src/shared/components/surfaces/Surface"
import Image from "next/image";
import { useState } from "react";
import Modal from "@/src/shared/components/modal/Modal";
import ImageGallery from "@/src/shared/components/inputs/image/gallery/ImageGallery";
import { Gender } from "@/src/shared/types/domain/value-objects";
import { Location } from "@/src/shared/types/domain/value-objects";
import BasicProfileForm from "../../auth/components/BasicProfileForm";
import { getValidBirthdateRange } from "@/src/shared/time";

export type ImageData = {
    id?: string,
    url: string,
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
        <div className={`relative max-w-max ${classNames}`}>
            {
                !isEmpty && <Image
                    className="rounded-full w-[100px] h-[100px] object-cover"
                    src={initialImages.find(image => image.order === 0)?.url ?? ""}
                    alt="User Image"
                    width={0}
                    height={0}
                    unoptimized={process.env.NODE_ENV === "development"}
                />
            }
            {
                isEmpty &&
                <div className="rounded-full w-[100px] h-[100px] flex flex-col items-center justify-center bg-gray-300">
                </div>
            }
            <button onClick={() => setModalOpen(prev => !prev)} className="bg-transparent absolute top-[50%] left-[50%] translate-x-[-50%] translate-y-[-50%] text-(length:--fs-1)">+</button>
            <Modal closed={() => setModalOpen(false)} hidden={!modalOpen} header="images">
                <ImageGallery<ImageData>
                    data={{ images: initialImages }}
                    onChange={
                        (newImages) => {
                            onChange?.(newImages.map((image, index) => ({ id: (image as ImageData)?.id, url: image.url, mimeType: image.mimeType, order: index })));
                        }
                    }
                />
            </Modal>
        </div>
    )
}

export type MyBasicInfoFormData = {
    alias?: string;
    birthDate?: string;
    images?: ImageData[];
    gender?: Gender;
    location?: Location;
}

export type MyBasicInfoFormError = { [P in keyof MyBasicInfoFormData]?: string }

export type MyBasicInfoFormProps = {
    className?: string
    data: MyBasicInfoFormData,
    error?: MyBasicInfoFormError,
    onChange: (data: MyBasicInfoFormData) => void
}
export default function MyBasicInfoForm({ className, data, error, onChange }: MyBasicInfoFormProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ")

    return (
        <Surface className={`${classNames} flex flex-col gap-3`} padding="none">
            <Surface className="flex flex-col gap-2" padding="sm" variant="secondary">
                <MyImagesForm className="shrink-0 mx-auto" initialImages={data.images ?? []} onChange={(images) => onChange({ ...data, images })} />
                <BasicProfileForm
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
                        location: data.location
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
        </Surface>
    )

}