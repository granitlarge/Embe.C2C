import React from "react";
import Image from "next/image";
import { DragDropProvider, useDraggable, useDroppable } from "@dnd-kit/react";

type ImageProps = {
    id: string;
    src: string;
    onRemove?: () => void;
}

function MyImage({ id, src, onRemove }: ImageProps) {
    const { ref: draggableRef } = useDraggable({
        id
    });
    const { ref: droppableRef } = useDroppable({
        id
    });
    return (
        <div ref={droppableRef}>
            <div ref={draggableRef} className="relative">
                <Image src={src} alt={"An Image"} className="w-40 h-50 object-cover rounded-lg" width={0} height={0} />
                <button onClick={onRemove} className="absolute top-0 right-0 -m-3 bg-gray-300 text-gray-600 rounded-full w-6 h-6 p-4 flex items-center justify-center">X</button>
            </div>
        </div>
    )
}

type ImageSelectorProps = {
    onImageSelected?: (src: string) => void;
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
                onImageSelected?.(imageSrc);
            }
            reader.readAsDataURL(file);
        }

    }

    function onClick() {
        inputRef.current!.value = "";
        inputRef.current!.click();
    }

    return (
        <div className="relative w-40 h-50 bg-gray-300 flex items-center justify-center cursor-pointer relative rounded-lg" onClick={onClick}>
            <input ref={inputRef} type="file" className="hidden" accept="image/*" onChange={onChange} />
            <span className="text-3xl text-gray-600">+</span>
        </div>

    )
}

export type Image = {
    src: string;
}

export type ImageGalleryProps = {
    value: Image[];
    className?: string;
    onChange?: (images: Image[]) => void;
    valid?: boolean;
    errorMessage?: string;
}

export default function ImageGallery({ value, className, onChange, valid, errorMessage }: ImageGalleryProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const imagesWithIds = value.map(image => ({ ...image, __id: crypto.randomUUID() }));
    return (
        <DragDropProvider
            onDragEnd={(event) => {
                if (event.canceled) return;
                const targetId = event.operation.target?.id;
                const sourceId = event.operation.source?.id;
                if (!targetId || !sourceId) return;
                const sourceIndex = imagesWithIds.findIndex(image => image.__id === sourceId);
                const targetIndex = imagesWithIds.findIndex(image => image.__id === targetId);
                if (sourceIndex === -1 || targetIndex === -1) return;
                const newValue = [...value];
                const [movedImage] = newValue.splice(sourceIndex, 1);
                newValue.splice(targetIndex, 0, movedImage);
                onChange?.(newValue);
            }}
        >
            <div className={`flex flex-wrap gap-4 ${classNames} w-full justify-center items-center`}>
                {
                    imagesWithIds.map((image, index) => (
                        <MyImage key={image.__id} id={image.__id} src={image.src} onRemove={() => onChange?.(value.filter((_, i) => i !== index))} />
                    ))
                }
                <ImageSelector onImageSelected={(src) => onChange?.([...value, { src }])} />
            </div>
            {!valid && errorMessage && <span className="error-message">{errorMessage}</span>}
        </DragDropProvider>
    )
}