"use client";

import Surface from "@/src/shared/components/surfaces/Surface"
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { Image as MyImage } from "@/src/shared/types/domain/entities";
import Image from "next/image";
import { useRef, useState } from "react";
import Modal from "@/src/shared/components/modal/Modal";
import ImageGallery from "@/src/shared/components/inputs/image/gallery/ImageGallery";
import Button from "@/src/shared/components/buttons/Button";
import { updateImages } from "../actions/action";

type ImageData = {
    id: string,
    url: string,
    mimeType: string,
}

type MyImagesFormProps = {
    initialImages: MyImage[]
}
function MyImagesForm({ initialImages }: MyImagesFormProps) {

    const [modalOpen, setModalOpen] = useState(false);
    const [images, setImages] = useState<{ id: string | undefined, url: string, mimeType: string, order: number }[]>
        (
            initialImages
                .sort((a, b) => a.imageDetails.order - b.imageDetails.order)
                .map(f => ({ id: f.id, url: f.imageDetails.url, mimeType: f.imageDetails.mimeType, order: f.imageDetails.order }))
        );
    const isEmpty = images.length === 0;

    async function onSave() {

        const imageAndIndex = images.map((image, index) => ({ image, index }));
        const imagesToKeep = imageAndIndex.filter(({ image }) => image.id !== undefined).map(({ image, index }) => ({ id: image.id!, order: index }));
        const imagesToCreate = imageAndIndex.filter(({ image }) => image.id === undefined);

        const response = await updateImages(
            imagesToKeep,
            imagesToCreate.map(({ image, index }) => ({ url: image.url, mimeType: image.mimeType, order: index }))
        );

        console.log(response);
        if (!response.success) {
            throw new Error("not implemented");
        }

        setImages(response.value?.map(image => ({ id: image.id, url: image.imageDetails.url, mimeType: image.imageDetails.mimeType, order: image.imageDetails.order })) ?? []);

    }

    return (
        <div className="relative max-w-max">
            {
                !isEmpty && <Image
                    className="rounded-full w-[100px] h-[100px] object-cover"
                    src={images.find(image => image.order === 0)?.url ?? ""}
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
                <ImageGallery
                    data={{ images }}
                    onChange={
                        (newImages) => {
                            setImages(newImages.map((image, index) => ({ id: (image as ImageData)?.id, url: image.url, mimeType: image.mimeType, order: index })));
                        }
                    }
                />
                <Button onClick={onSave}>Save</Button>
            </Modal>
        </div>
    )
}

export type MyBasicInfoFormProps = {
    className?: string
    user: ReadDto<User, UserPermission>
}
export default function MyBasicInfoForm({ className, user }: MyBasicInfoFormProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ")

    return (
        <Surface className={`${classNames}`} padding="none">
            <MyImagesForm initialImages={user.data.images ?? []} />
        </Surface>
    )

}