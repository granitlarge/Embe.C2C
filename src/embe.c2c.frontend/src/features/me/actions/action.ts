"use server";

import { Mutate } from "@/src/shared/apis/api";
import { ApiResponse, } from "@/src/shared/apis/type";
import { Guid } from "@/src/shared/cache";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { Gender, Location } from "@/src/shared/types/domain/value-objects";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { ImageData } from "../components/MyInfoForm";
import { AddImagesResult } from "./type";
import { clearTokens, getRefreshToken } from "@/src/shared/security/functions";
import { getAuthenticatedUser } from "@/src/shared/user";

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
): Promise<ApiResponse<AddImagesResult>> {

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

    const response = Mutate<AddImagesResult>(
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

export async function logout(): Promise<ApiResponse<void>> {

    const response = await Mutate<void>(
        `${process.env.API_URL}/api/auth/signout`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ refreshToken: await getRefreshToken() })
        }
    );

    if (response.success) {
        await clearTokens();
    }

    return response;

}

export async function deleteAccount(): Promise<ApiResponse<void>> {

    const userId = (await getAuthenticatedUser())?.userId;

    const response = await Mutate<void>(
        `${process.env.API_URL}/api/user`,
        {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ userId: userId })
        }
    );

    if (response.success) {
        await clearTokens();
    }

    return response;

}

export async function resetPassword(newPassword: string) {

    const response = await Mutate<void>
        (
            `${process.env.API_URL}/api/auth/reset-password`,
            {
                method: "POST",
                body: JSON.stringify({ newPassword }),
                headers: {
                    "Content-Type": "application/json"
                }
            }
        )

    return response;

}

export async function changeEmail(newEmail: string, verificationCode: string) : Promise<ApiResponse<void>> {
    const response = await Mutate<void>(
        `${process.env.API_URL}/api/user/change-email`,
        {
            method: "POST",
            body: JSON.stringify({ newEmail, verificationCode }),
            headers: {
                "Content-Type": "application/json"
            }
        }
    )
    return response;
}
