"use client";
import ImageCropper from "@/src/shared/components/images/ImageCropper";
import LargeModal from "@/src/shared/components/modal/LargeModal";

export default function DesignPage() {

    return (
        <div className="p-10">
            <ImageCropper src={"./test.jpg"} width={600} height={800} />
        </div>
    )
}