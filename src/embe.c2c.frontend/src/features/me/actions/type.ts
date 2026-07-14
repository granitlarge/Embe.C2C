import { Image } from "@/src/shared/types/domain/entities";

export type AddImageResult = {
    uploadUrl?: string;
    image: Image;
}