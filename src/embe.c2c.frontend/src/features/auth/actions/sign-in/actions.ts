"use server";

import { SendRequest } from "@/src/shared/api";
import { Credentials } from "@/src/shared/types/application/types";
import { AccessTokenName, RefreshTokenName } from "@/src/shared/security/constants";
import { saveToken } from "@/src/shared/security/functions";
import { SignInError } from "./types";

export async function SignIn(username: string, password: string): Promise<SignInError | undefined> {

    const response = await SendRequest<Credentials, SignInError>(new Request(`${process.env.API_URL}/api/auth/signin`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ username, password })
    }), false);

    if (response.success) {
        await Promise.all([
            saveToken(AccessTokenName, response.value!.accessToken),
            saveToken(RefreshTokenName, response.value!.refreshToken)
        ]);
    }

    return response.success ? undefined : response.error;

}