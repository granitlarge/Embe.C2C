"use client";
import Button from "@/src/shared/components/buttons/Button";
import ImageCropper from "@/src/shared/components/images/ImageCropper";
import Cropper from 'react-easy-crop'

export default function DesignPage() {

    return (
        <div className="flex h-[100vh] justify-center items-center">
            <Button intent="save" onClick={() => {
                return new Promise((resolve, reject) => {
                    setTimeout(() => {
                        resolve();
                    }, 1000)
                })
            }}>hello</Button>
        </div>
    )
}