"use client";

import { useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import Image from "@/src/shared/components/images/Image";
import styles from "./ImageGallery.module.css";
import Button from "../buttons/Button";

export type PaginationDotsProps = {
    total: number,
    current: number
    className?: string;
}
export function PaginationDots({ total, current, className }: PaginationDotsProps) {

    const classNames = [className].filter(Boolean).join(" ");
    return (
        <div className={`${classNames} rounded-md flex gap-1 justify-center overflow-x-hidden`}>
            {
                Array.from({ length: total }, (_, index) => (
                    <div key={index} className={`rounded-full ${index === current ? "bg-(--universal-primary-bg)" : "bg-(--universal-primary-fc)"} w-1 h-1`}></div>
                ))
            }
        </div>
    )

}

type ImageGalleryImageProps = {
    src: string;
    alt: string;
    className?: string;
}
function ImageGalleryImage({ src, alt, className }: ImageGalleryImageProps) {
    const classNames = [className].filter(Boolean).join(" ");
    return (
        <Image
            src={src}
            alt={alt}
            className={`${classNames} ${styles.image} rounded-md`}
            width={700}
            height={1400}
            unoptimized={process.env.NODE_ENV === "development"}
        />
    )
}

export type ImageGalleryProps = {
    imageUrls: (string | undefined)[];
    className?: string;
}
export default function ImageGallery({ className, imageUrls }: ImageGalleryProps) {

    const classNames = [className].filter(Boolean).join(" ");
    const [currentImageUrlIndex, setCurrentImageUrlIndex] = useState(0);
    const currentImageUrl = imageUrls[currentImageUrlIndex];

    return (
        <div className={`${classNames} relative flex flex-col items-center justify-center`}>
            {
                imageUrls.filter(imageUrl => !!imageUrl).map((imageUrl, index) => (
                    <ImageGalleryImage key={index} className={`w-full h-full ${index === currentImageUrlIndex ? "" : "hidden"}`} src={imageUrl!} alt={`Image ${currentImageUrlIndex + 1}`} />
                ))
            }
            {
                !currentImageUrl &&
                <div className="rounded-md w-full h-full flex flex-col items-center justify-center bg-gray-300">
                    <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">no images</span>
                </div>
            }
            {
                imageUrls.length > 1 &&
                <>
                    <Button intent="navigate" onClick={() => setCurrentImageUrlIndex(prev => (prev - 1 + imageUrls.length) % imageUrls.length)} className="rounded-full absolute -left-3 top-1/2 transform max-w-max -translate-y-1/2 p-0 bg-gray-300"><ChevronLeft className="text-(--primary-fc) h-[calc(2*var(--primary-fs))] w-[calc(2*var(--primary-fs))]" /></Button>
                    <Button intent="navigate" onClick={() => setCurrentImageUrlIndex(prev => (prev + 1) % imageUrls.length)} className="rounded-full absolute -right-3 top-1/2 transform max-w-max -translate-y-1/2 p-0 bg-gray-300"><ChevronRight className="text-(--primary-fc) h-[calc(2*var(--primary-fs))] w-[calc(2*var(--primary-fs))]" /></Button>
                    <PaginationDots total={imageUrls.length} current={currentImageUrlIndex} className="absolute top-3 left-1/2 transform -translate-x-1/2" />
                </>
            }
            {
                imageUrls.length > 1 &&
                <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[80%] h-full flex justify-center ">
                    <Button className="p-0 w-[40%] h-full" onClick={() => setCurrentImageUrlIndex((prev) => (prev - 1 + imageUrls.length) % imageUrls.length)}></Button>
                    <Button className="p-0 w-[40%] h-full" onClick={() => setCurrentImageUrlIndex(prev => (prev + 1) % imageUrls.length)}></Button>
                </div>
            }
        </div>
    )

}