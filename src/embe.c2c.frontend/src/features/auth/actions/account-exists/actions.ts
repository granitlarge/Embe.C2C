"use server";

import { ApiResponse, FailureReason, Read } from "@/src/shared/api";

export async function accountExists(email: string): Promise<ApiResponse<boolean, FailureReason>> {

    const result = await Read<boolean>
    (
        `${process.env.API_URL}/api/auth/account-exists`,
        {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ email }),
        },
        false
    );

    return result;

}