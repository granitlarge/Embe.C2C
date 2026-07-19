import React, { useState } from "react";
import { DragDropProvider, useDraggable, useDroppable } from "@dnd-kit/react";
import Image from "@/src/shared/components/images/Image";
import Surface from "../../../surfaces/Surface";
import { X } from "@deemlol/next-icons";
import ErrorMessage from "../../ErrorMessage";
import { ImageStatus } from "@/src/shared/types/domain/value-objects";
import Button from "../../../buttons/Button";
import LargeModal from "../../../modal/LargeModal";
import ImageCropGallery from "../crop/ImageCrop";

type MyImageProps = {
    id: string;
    src?: string;
    status?: ImageStatus
    onRemove?: () => void;
}
function MyImage({ id, src, status, onRemove }: MyImageProps) {
    const { ref: draggableRef, isDragging } = useDraggable({
        id
    });
    const { ref: droppableRef } = useDroppable({
        id
    });
    return (
        <div ref={droppableRef}>
            <div ref={draggableRef} className="relative">
                {
                    src && src !== "" &&
                    <Image
                        src={src}
                        alt={"An Image"}
                        className={`w-20 h-30 object-cover rounded-lg ${isDragging ? "shadow-2xl shadow-black" : ""}`}
                        width={100}
                        height={100}
                        unoptimized={process.env.NODE_ENV === "development"}
                    />
                }
                {
                    status === ImageStatus.Pending &&
                    <Surface className="flex items-center justify-center absolute top-0 left-0 w-20 h-30 opacity-80 rounded-lg" variant="secondary">
                        <span className="mx-auto text-(--primary-fc) text-(length:--tertiary-fs)">awaiting approval</span>
                    </Surface>
                }
                <Button onClick={onRemove} className="button bg-gray-300 absolute top-0 right-0 -m-3 rounded-full max-w-max max-h-max flex items-center justify-center">
                    <X className="text-(--primary-fc) w-[12px] h-[12px]" />
                </Button>
            </div>
        </div>
    )
}

type ImageSelectorProps = {
    onImageSelected?: (image: { url: string, mimeType: string, crop: { x: number, y: number, width: number, height: number } }[]) => void;
    className?: string;
}
function ImageSelector({ className, onImageSelected }: ImageSelectorProps) {

    const inputRef = React.useRef<HTMLInputElement>(null);
    const [showCropper, setShowCropper] = useState(false);
    const [images, setImages] = useState([] as { url: string, mimeType: string }[]);

    function onChange(event: React.ChangeEvent<HTMLInputElement>) {
        const target = event.target as HTMLInputElement;
        if (target.files && target.files.length > 0) {
            let images = Array.from(target.files).map(f => ({ url: URL.createObjectURL(f), mimeType: f.type }));
            setImages(images);
            setShowCropper(true);
        }
    }

    function onClick() {
        inputRef.current!.value = "";
        inputRef.current!.click();
    }

    return (
        <Surface className={`relative w-20 h-30 flex items-center justify-center cursor-pointer relative rounded-lg ${className}`} onClick={onClick}
            variant="tertiary">
            <input ref={inputRef} type="file" multiple className="hidden" accept="image/*" onChange={onChange} />
            <span className="text-3xl text-(--secondary-fc)">+</span>
            {
                showCropper &&
                <LargeModal
                    className="p-3"
                    header="crop"
                    hidden={false}
                    closed={() => setShowCropper(false)}
                >
                    <ImageCropGallery images={images.map(i => i.url)} onChange={(crops) => {
                        const result = images.map((image, index) => ({
                            ...image,
                            crop: crops[index]
                        }))
                        onImageSelected?.(result);
                        setShowCropper(false);
                    }} />
                </LargeModal>
            }
        </Surface>
    )
}

export type Image = {
    url?: string;
    largeUrl?: string;
    mediumUrl?: string;
    smallUrl?: string;
    mimeType: string;
    status?: ImageStatus;
    crop?: {
        x: number,
        y: number,
        width: number,
        height: number
    }
}

export type ImageGalleryInputData<T extends Image = Image> = {
    images: T[];
}

export type ImageGalleryInputError = { [P in keyof ImageGalleryInputData]?: string };

export type ImageGalleryInputProps<T extends Image = Image> = {
    data?: ImageGalleryInputData<T>;
    error?: ImageGalleryInputError;
    className?: string;
    onChange?: (images: (T | Image)[]) => void;
}
export default function ImageGalleryInput<T extends Image = Image>({ data, error, className, onChange }: ImageGalleryInputProps<T>) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const images = (data?.images ?? []).map((image) => ({ ...image, __id: crypto.randomUUID() }));
    return (
        <DragDropProvider
            onDragEnd={(event) => {
                if (event.canceled) return;
                const targetId = event.operation.target?.id;
                const sourceId = event.operation.source?.id;
                if (!targetId || !sourceId) return;
                const sourceIndex = images.findIndex(image => image.__id === sourceId);
                const targetIndex = images.findIndex(image => image.__id === targetId);
                if (sourceIndex === -1 || targetIndex === -1) return;
                const [movedImage] = images.splice(sourceIndex, 1);
                images.splice(targetIndex, 0, movedImage);
                onChange?.(images.map(({ __id, ...image }) => image));
            }}
        >
            <div className={`flex flex-wrap gap-4 ${classNames} w-full justify-center items-center p-2`}>
                {
                    images.map((image, index) => (
                        <MyImage
                            key={image.__id}
                            id={image.__id}
                            src={image.smallUrl ?? image.mediumUrl ?? image.largeUrl ?? image.url}
                            status={image.status}
                            onRemove={() => onChange?.(images.filter((_, i) => i !== index).map(({ __id, ...image }) => image))}
                        />
                    ))
                }
                <ImageSelector onImageSelected={(image) => onChange?.([...images.map(({ __id, ...image }) => image), ...image])} />
            </div>
            <ErrorMessage message={error?.images} />
        </DragDropProvider>
    )

}