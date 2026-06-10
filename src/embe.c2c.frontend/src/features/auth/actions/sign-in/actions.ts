"use server";

import { Mutate } from "@/src/shared/api";
import { Credentials } from "@/src/shared/types/application/types";
import { SignInError } from "./types";
import { saveAccessToken, saveRefreshToken } from "@/src/shared/security/functions";

export async function SignIn(email: string, password: string): Promise<SignInError | undefined> {

    const response = await Mutate<Credentials, SignInError>
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

    return response.success ? undefined : response.reason;

}