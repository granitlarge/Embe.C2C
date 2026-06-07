import ImageGallery from "@/src/shared/components/inputs/image/gallery/ImageGallery";
import { FileDetails } from "@/src/shared/types/domain/value-objects";

export type ImagesFormData = {
    images: FileDetails[];
    imagesError?: string;
}

export type ImagesFormProps = {
    data: ImagesFormData;
    onChange: (data: ImagesFormData) => void;
}

export default function ImagesForm({ data, onChange }: ImagesFormProps) {
    return (
        <div className="form flex flex-col gap-3 w-full items-center">
            <ImageGallery value={data.images} onChange={(newImages) => onChange({ ...data, images: newImages })} valid={data.imagesError === undefined} errorMessage={data.imagesError} />
        </div>
    )
}