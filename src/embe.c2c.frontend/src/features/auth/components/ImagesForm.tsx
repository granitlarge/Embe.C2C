import ImageGallery from "@/src/shared/components/inputs/image/gallery/ImageGallery";
import { CreateFile } from "@/src/shared/types/dtos/types";


export type ImagesFormData = {
    images: CreateFile[];
}

export type ImagesFormError = { [P in keyof ImagesFormData]?: string };

export type ImagesFormProps = {
    data?: ImagesFormData;
    error?: ImagesFormError;
    onChange: (data: ImagesFormData) => void;
}

export default function ImagesForm({ data, error, onChange }: ImagesFormProps) {
    return (
        <div className="form flex flex-col gap-3 w-full items-center">
            <ImageGallery data={data} error={error} onChange={(newImages) => onChange({ ...data, images: newImages.map((image, index) => ({ ...image, order: index })) })} />
        </div>
    )
}