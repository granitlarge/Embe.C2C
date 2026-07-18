import { useState } from "react";
import Cropper from "react-easy-crop"
import ImageCropper from "../../../images/ImageCropper";
import { PaginationDots } from "../../../images/ImageGallery";

export type ImageCrop = {
    images: string[];
    onChange?: () => void;
}
export default function ImageCrop({ images }: ImageCrop) {

    if (images.length === 0) {
        throw new Error("images.length must be greater than 0.");
    }

    const [index, setIndex] = useState(0);

    return (
        <div className="relative">

            <ImageCropper
                onCrop={() => setIndex(prev => (prev + 1) % images.length)}
                src={images[index]}
                width={5000}
                height={5000}

            />
            {
                images.length > 1 && 
                <PaginationDots className="absolute top-3 left-1/2 -translate-x-1/2" current={1} total={images.length} />
            }

        </div>
    )
}