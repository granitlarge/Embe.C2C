import React, { useState } from "react";
import Image from "next/image";
import { DragDropProvider, useDraggable, useDroppable } from "@dnd-kit/react";
import Surface from "../../../surfaces/Surface";
import { X } from "@deemlol/next-icons";

type MyImageProps = {
    id: string;
    src: string;
    isDragging?: boolean;
    onRemove?: () => void;
}

function MyImage({ id, src, onRemove, isDragging = false }: MyImageProps) {
    const { ref: draggableRef } = useDraggable({
        id
    });
    const { ref: droppableRef } = useDroppable({
        id
    });
    return (
        <div ref={droppableRef}>
            <div ref={draggableRef} className="relative">
                <Image src={src} alt={"An Image"} className={`w-30 h-40 object-cover rounded-lg ${isDragging ? "shadow-2xl" : ""}`} width={0} height={0} unoptimized={process.env.NODE_ENV === "development"} />
                <button onClick={onRemove} className="bg-gray-300 absolute top-0 right-0 -m-3 rounded-full w-6 h-6 flex items-center justify-center">
                    <X className="text-black" />
                </button>
            </div>
        </div>
    )
}

type ImageSelectorProps = {
    onImageSelected?: (image: { url: string, mimeType: string }) => void;
}

function ImageSelector({ onImageSelected }: ImageSelectorProps) {

    const inputRef = React.useRef<HTMLInputElement>(null);

    function onChange(event: React.ChangeEvent<HTMLInputElement>) {
        const target = event.target as HTMLInputElement;
        if (target.files && target.files[0]) {
            const file = target.files[0];
            const reader = new FileReader();
            reader.onload = (e) => {
                const imageSrc = e.target?.result as string;
                onImageSelected?.({ url: imageSrc, mimeType: file.type });
            }
            reader.readAsDataURL(file);
        }

    }

    function onClick() {
        inputRef.current!.value = "";
        inputRef.current!.click();
    }

    return (
        <Surface
            className="relative w-30 h-40 flex items-center justify-center cursor-pointer relative rounded-lg" onClick={onClick}
            variant="tertiary">
            <input ref={inputRef} type="file" className="hidden" accept="image/*" onChange={onChange} />
            <span className="text-3xl text-(--secondary-fc)">+</span>
        </Surface>

    )
}

export type Image = {
    url: string;
    mimeType: string;
}

export type ImageGalleryData<T extends Image = Image> = {
    images: T[];
}
export type ImageGalleryError = { [P in keyof ImageGalleryData]?: string };

export type ImageGalleryProps<T extends Image = Image> = {
    data?: ImageGalleryData<T>;
    error?: ImageGalleryError;
    className?: string;
    onChange?: (images: (T | Image)[]) => void;
}

export default function ImageGallery<T extends Image = Image>({ data, error, className, onChange }: ImageGalleryProps<T>) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const [images, setImages] = useState(data?.images.map(image => ({ ...image, __id: crypto.randomUUID(), __isDragging: false })) ?? []);
    return (
        <DragDropProvider
            onDragStart={(event) => {
                const sourceId = event.operation.source?.id;
                setImages(prev => prev.map(image => image.__id === sourceId ? { ...image, __isDragging: true } : image));
            }}
            onDragEnd={(event) => {
                setImages(prev => prev.map(image => image.__isDragging ? { ...image, __isDragging: false } : image));
                if (event.canceled) return;
                const targetId = event.operation.target?.id;
                const sourceId = event.operation.source?.id;
                if (!targetId || !sourceId) return;
                const sourceIndex = images.findIndex(image => image.__id === sourceId);
                const targetIndex = images.findIndex(image => image.__id === targetId);
                if (sourceIndex === -1 || targetIndex === -1) return;
                const [movedImage] = images.splice(sourceIndex, 1);
                images.splice(targetIndex, 0, movedImage);
                onChange?.(images.map(({ __id, __isDragging, ...image }) => image));
            }}
        >
            <div className={`flex flex-wrap gap-4 ${classNames} w-full justify-center items-center`}>
                {
                    images.map((image, index) => (
                        <MyImage key={image.__id} id={image.__id} src={image.url} isDragging={image.__isDragging} onRemove={() => onChange?.(images.filter((_, i) => i !== index).map(({ __id, __isDragging, ...image }) => image))} />
                    ))
                }
                <ImageSelector onImageSelected={(image) => onChange?.([...images.map(({ __id, __isDragging, ...image }) => image), image])} />
            </div>
            {error?.images && <span className="text-(--error-fc)">{error.images}</span>}
        </DragDropProvider>
    )
}