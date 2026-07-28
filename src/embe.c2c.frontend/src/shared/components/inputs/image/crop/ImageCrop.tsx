import { useRef, useState } from "react";
import ImageCropper from "../../../images/ImageCropper";
import { PaginationDots } from "../../../images/ImageGallery";

export type ImageCropGallery = {
    images: string[];
    onChange?: (crops: { x: number, y: number, width: number, height: number }[]) => void;
}
export default function ImageCropGallery({ onChange, images }: ImageCropGallery) {

    if (images.length === 0) {
        throw new Error("images.length must be greater than 0.");
    }

    const cropsRef = useRef([] as { x: number, y: number, width: number, height: number }[]);
    const [index, setIndex] = useState(0);

    function onCrop(crop: { x: number, y: number, width: number, height: number }) {
        cropsRef.current.push(crop);
        if (index == images.length - 1) {
            onChange?.(cropsRef.current);
            setIndex(0);
            cropsRef.current = [];
            setIndex(0);
        } else {
            setIndex(prev => prev + 1);
        }
    }

    return (
        <div className="relative">

            <ImageCropper
                onCrop={onCrop}
                src={images[index]}
                aspect={1/1}
            />
            {
                images.length > 1 &&
                <PaginationDots className="absolute top-3 left-1/2 -translate-x-1/2" current={index} total={images.length} />
            }
        </div>
    )

}