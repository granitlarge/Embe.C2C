import { useState } from "react";
import Cropper from "react-easy-crop"
import ImageCropper from "../../../images/ImageCropper";

export type ImageCrop = {
    images: string[];
    onChange?: () => void;
}
export default function ImageCrop({ images }: ImageCrop) {

    if (images.length === 0) {
        throw new Error("images.length must be greater than 0.");
    }

    const [crops, setCrops] = useState<{ x: number, y: number }[]>([{ x: 0, y: 0 }]);
    const [zoom, setZoom] = useState(1);
    const [index, setIndex] = useState(0);

    function onCropComplete() {

    }

    return (
        <div className="w-full">
            <div className="relative w-full">
                <ImageCropper
                    src={images[index]}
                    height={600}
                    width={800}
                />
            </div>
        </div>
    )
}