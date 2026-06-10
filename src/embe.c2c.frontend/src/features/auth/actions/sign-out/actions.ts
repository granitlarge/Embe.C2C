"use server";

import { Mutate } from "@/src/shared/api";

export async function Logout(): Promise<void> {
    await Mutate(`${process.env.API_URL}/api/auth/logout`, {
        method: "POST"
    });
}