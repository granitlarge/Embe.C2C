"use server";

import { RegisterRequest } from "./types";
import { Mutate } from "@/src/shared/apis/api";
import { Credentials } from "@/src/shared/types/application/types";
import { saveAccessToken, saveRefreshToken } from "@/src/shared/security/functions";
import { Error } from "@/src/shared/apis/type";

export async function register(request: RegisterRequest): Promise<Error[] | undefined> {

    const response = await Mutate<Credentials>
        (
            `${process.env.API_URL}/api/user/register`,
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(request),
            },
            false
        );

    if (response.success) {
        await Promise.all([
            saveAccessToken(response.value!.accessToken),
            saveRefreshToken(response.value!.refreshToken)
        ]);
    }

    return response.success ? undefined : response.errors;

}