"use server";

import { ApiResponse, FailureReason, SendRequest } from "@/src/shared/api";
import { User } from "@/src/shared/types/domain/aggregates";
import { RegisterRequest } from "./types";

export async function register(request: RegisterRequest): Promise<ApiResponse<User, FailureReason>> {

    const response = await SendRequest<User>(new Request(`${process.env.API_URL}/api/user/register`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(request)
    }), false);

    return response;

}