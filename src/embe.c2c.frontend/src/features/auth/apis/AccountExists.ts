"use server";

import { SendRequest } from "@/src/shared/api";

export async function checkAccountExists(email: string): Promise<boolean> {
    const result = await SendRequest<boolean>(new Request(`${process.env.API_URL}/api/auth/account-exists`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(email)
    }), false);
    return result;
}