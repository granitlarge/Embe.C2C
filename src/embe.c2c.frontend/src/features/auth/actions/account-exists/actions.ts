"use server";

import { Read } from "@/src/shared/apis/api";
import { ApiResponse, FailureReason,  } from "@/src/shared/apis/type";

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