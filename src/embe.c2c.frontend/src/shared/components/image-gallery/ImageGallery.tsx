import { useState } from "react";
import Image from "next/image";
import { ChevronLeft, ChevronRight } from "lucide-react";

type ImageGalleryImageProps = {
    src: string;
    alt: string;
    className?: string;
}
function ImageGalleryImage({ src, alt, className }: ImageGalleryImageProps) {
    const classNames = [className].filter(Boolean).join(" ");
    return (
        <Image src={src} alt={alt} className={classNames} width={0} height={0} unoptimized={process.env.NODE_ENV === "development"} />
    )
}

export type ImageGalleryProps = {
    imageUrls: string[];
    className?: string;
}
export default function ImageGallery({ className, imageUrls }: ImageGalleryProps) {

    const classNames = [className].filter(Boolean).join(" ");
    const [currentImageUrlIndex, setCurrentImageUrlIndex] = useState(0);
    const currentImageUrl = imageUrls[currentImageUrlIndex];

    return (
        <div className={`${classNames} relative`}>
            <ImageGalleryImage className="w-full h-auto" src={currentImageUrl} alt={`Image ${currentImageUrlIndex + 1}`} />
            <button onClick={() => setCurrentImageUrlIndex(prev => Math.max(prev - 1, 0))} className="rounded-full absolute -left-3 top-1/2 transform max-w-max -translate-y-1/2 p-0 bg-gray-300"><ChevronLeft className="text-(--primary-fc) h-[calc(2*var(--primary-fs))] w-[calc(2*var(--primary-fs))]" /></button>
            <button onClick={() => setCurrentImageUrlIndex(prev => Math.min(prev + 1, imageUrls.length - 1))} className="rounded-full absolute -right-3 top-1/2 transform max-w-max -translate-y-1/2 p-0 bg-gray-300"><ChevronRight className="text-(--primary-fc) h-[calc(2*var(--primary-fs))] w-[calc(2*var(--primary-fs))]" /></button>
        </div>
    )

}