"use client";
import ImageCrop from "@/src/shared/components/inputs/image/crop/ImageCrop";
import LargeModal from "@/src/shared/components/modal/LargeModal";

export default function DesignPage() {

    return (
        <LargeModal hidden={false} closed={() => { }}>
            <ImageCrop images={["./test.jpg"]} />
        </LargeModal>
    )
}