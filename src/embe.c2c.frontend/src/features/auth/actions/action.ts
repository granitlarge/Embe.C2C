"use server";

import { Mutate, Read } from "@/src/shared/apis/api";
import { ApiResponse } from "@/src/shared/apis/type";
import { NullGuid } from "@/src/shared/cache";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { getAuthenticatedUser } from "@/src/shared/user";

export async function getMe(): Promise<ApiResponse<ReadDto<User, UserPermission>>> {

    const currentUser = await getAuthenticatedUser();
    const response = await Read<ReadDto<User, UserPermission>>(`${process.env.API_URL}/api/user/me`, {
        method: "GET",
        next: {
            tags: [`user:${currentUser?.userId || NullGuid}`]
        }
    });

    return response;

}

export async function sendResetPasswordEmail(email: string): Promise<ApiResponse<void>> {

    const response = await Mutate<void>
        (
            `${process.env.API_URL}/api/auth/forgot-password`,
            {
                method: "POST",
                body: JSON.stringify({ email }),
                headers: {
                    "Content-Type": "application/json"
                }
            },
            false
        )

    return response;

}

export async function resetPassword(token: string, newPassword: string): Promise<ApiResponse<void>> {
    const response = await Mutate<void>
        (
            `${process.env.API_URL}/api/auth/reset-password`,
            {
                method: "POST",
                body: JSON.stringify({ newPassword }),
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                }
            },
            false
        )

    return response;
}