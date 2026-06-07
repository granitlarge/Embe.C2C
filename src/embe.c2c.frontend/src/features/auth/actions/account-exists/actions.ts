"use server";

import { ApiResponse, FailureReason, SendRequest } from "@/src/shared/api";

export async function accountExists(email: string): Promise<ApiResponse<boolean, FailureReason>> {
    const result = await SendRequest<boolean>(new Request(`${process.env.API_URL}/api/auth/account-exists`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ email })
    }), false);

    return result;
}