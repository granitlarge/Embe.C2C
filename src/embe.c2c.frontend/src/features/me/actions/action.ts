"use server";

import { Mutate } from "@/src/shared/apis/api";
import { ApiResponse, FailureReason, } from "@/src/shared/apis/type";
import { Guid } from "@/src/shared/cache";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { Gender, Location } from "@/src/shared/types/domain/value-objects";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { ImageData } from "../components/MyInfoForm";

export async function updateProfile
    (
        userId: Guid,
        alias: string,
        birthDate: string,
        gender?: Gender,
        location?: Location,
        imagesToKeep?: { id: string, order: number }[],
        bio?: string
    ): Promise<ApiResponse<ReadDto<User, UserPermission>>> {

    const body = JSON.stringify({ userId, alias, birthDate, gender, location, imagesToKeep, bio });
    const response = Mutate<ReadDto<User, UserPermission>>(
        `${process.env.API_URL}/api/user`,
        {
            method: "PUT",
            body: body,
            headers: {
                "Content-Type": "application/json"
            }
        }
    );

    return response;

}

export async function addImages(
    images: { image: ImageData, base64Data: string }[]
): Promise<ApiResponse<ReadDto<User, UserPermission>>> {

    var body = JSON.stringify({
        images: images.map((i, index) => {
            return {
                base64EncodedImageData: i.base64Data,
                mimeType: i.image.mimeType,
                order: i.image.order,
                cropOffsetX: i.image.crop?.x,
                cropOffsetY: i.image.crop?.y,
                width: i.image.crop?.width,
                height: i.image.crop?.height,
            }
        })
    });

    const response = Mutate<ReadDto<User, UserPermission>>(
        `${process.env.API_URL}/api/user/upload-images`,
        {
            method: "POST",
            body: body,
            headers: {
                "Content-Type": "application/json",
            }
        }
    )

    return response;

}