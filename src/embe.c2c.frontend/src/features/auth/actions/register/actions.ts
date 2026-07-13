"use server";

import { RegisterRequest, RegisterUserFailureReason } from "./types";
import { ApiResponse } from "@/src/shared/apis/type";
import { Mutate } from "@/src/shared/apis/api";

export async function register(request: RegisterRequest): Promise<ApiResponse<void, RegisterUserFailureReason>> {

    const response = await Mutate<void, RegisterUserFailureReason>
        (
            `${process.env.API_URL}/api/user/register`,
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(request)
            },
            false
        );

    return response;

}