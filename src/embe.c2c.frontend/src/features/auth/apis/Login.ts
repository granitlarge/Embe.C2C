"use server";

import { SendRequest } from "@/src/shared/api";
import { LoginResponse } from "../types/login-response";

export async function Login(username: string, password: string): Promise<LoginResponse> {
    return await SendRequest<LoginResponse>(new Request(`${process.env.API_URL}/api/auth/login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ username, password })
    }));
}