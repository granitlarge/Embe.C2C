"use server";

import { cookies } from "next/headers";
import { AccessTokenName, RefreshTokenName, TokenCookieOptions } from "./constants";
import { ApiResponse, FailureReason } from "../api";

export type Token = {
    token: string;
    expiresAt: string;
}

export type RefreshToken = Token & {
    id: string;
}

export async function saveAccessToken(token: Token) {
    await saveToken(AccessTokenName, token);
}

export async function saveRefreshToken(token: RefreshToken) {
    await saveToken(RefreshTokenName, token);
}

async function saveToken(name: string, token: Token) {
    const cookie = await cookies();
    cookie.set(name, token.token, { ...TokenCookieOptions, expires: new Date(token.expiresAt) });
}

async function deleteToken(name: typeof AccessTokenName | typeof RefreshTokenName) {
    const cookie = await cookies();
    cookie.delete(name);
}

export async function getRefreshToken(): Promise<string | undefined> {
    const cookie = await cookies();
    const refreshToken = cookie.get(RefreshTokenName)?.value;
    return refreshToken;
}

export async function getAccessToken() {
    const cookie = await cookies();
    const accessToken = cookie.get(AccessTokenName)?.value;
    return accessToken;
}

export async function clearTokens() {
    await deleteToken(AccessTokenName);
    await deleteToken(RefreshTokenName);
}

export type RefreshAccessTokenResponse = ApiResponse<{ accessToken: Token }, FailureReason>;
export async function refreshAccessToken(): Promise<Token | undefined> {

    const refreshToken = await getRefreshToken();
    if (!refreshToken) {
        return undefined;
    }

    const response = await fetch(`${process.env.API_URL}/api/auth/refresh`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${refreshToken}`
        },
        body: JSON.stringify({ refreshToken })
    });

    if (!response.ok) {
        return undefined;
    }

    const responseBody = await response.json();
    const refreshAccessTokenResponse = responseBody as RefreshAccessTokenResponse;
    return refreshAccessTokenResponse.value?.accessToken;

}