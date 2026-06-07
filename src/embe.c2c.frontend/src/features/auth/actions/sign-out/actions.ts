"use server";

import { SendRequest } from "@/src/shared/api";

export async function Logout(): Promise<void> {
    await SendRequest(new Request(`${process.env.API_URL}/api/auth/logout`, {
        method: "POST"
    }));
}