"use server";

import { Mutate } from "@/src/shared/api";
import { clearTokens, getRefreshToken } from "@/src/shared/security/functions";

export async function signOut(): Promise<void> {
    const refreshToken = await getRefreshToken();
    const response = await Mutate<void>(`${process.env.API_URL}/api/auth/signout`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({ refreshToken }),
    });

    if (response.success) {
        await clearTokens();
    }
}