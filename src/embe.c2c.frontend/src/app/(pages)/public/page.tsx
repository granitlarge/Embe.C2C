"use client";
import ImageCropper from "@/src/shared/components/images/ImageCropper";

export default function DesignPage() {

    return (
        <ImageCropper src={"./test.jpg"} width={600} height={800} />
    )
}