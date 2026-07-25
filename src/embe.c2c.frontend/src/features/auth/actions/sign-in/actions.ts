"use server";

import { Credentials } from "@/src/shared/types/application/types";
import { SignInError } from "./types";
import { saveAccessToken, saveRefreshToken } from "@/src/shared/security/functions";
import { Mutate } from "@/src/shared/apis/api";
import { Error } from "@/src/shared/apis/type";

export async function signIn(email: string, password: string): Promise<Error[] | undefined> {

    const response = await Mutate<Credentials>
        (
            `${process.env.API_URL}/api/auth/signin`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ email, password })
        }, false);

    if (response.success) {
        await Promise.all([
            saveAccessToken(response.value!.accessToken),
            saveRefreshToken(response.value!.refreshToken)
        ]);
    }

    return response.success ? undefined : response.errors;

}