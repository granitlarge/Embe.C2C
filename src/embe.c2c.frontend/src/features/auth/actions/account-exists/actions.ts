"use server";

import { ApiResponse, FailureReason, Read } from "@/src/shared/api";

export async function accountExists(email: string): Promise<ApiResponse<boolean, FailureReason>> {

    const result = await Read<boolean>
    (
        `${process.env.API_URL}/api/auth/account-exists?email=${encodeURIComponent(email)}`,
        {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
        },
        false
    );

    return result;

}