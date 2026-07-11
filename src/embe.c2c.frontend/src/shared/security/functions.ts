"use server";

import { cookies } from "next/headers";
import { AccessTokenName, RefreshTokenName, TokenCookieOptions } from "./constants";
import * as jwtdecode from "jwt-decode";
import { RefreshAccessTokenResponse, RefreshToken, Token } from "./types";

export async function saveAccessToken(token: Token) {
    await saveToken(AccessTokenName, token);
}

export async function saveRefreshToken(token: RefreshToken) {
    await saveToken(RefreshTokenName, token);
}

async function saveToken(name: string, token: Token) {
    const cookie = await cookies();
    cookie.set(name, token.token, { ...TokenCookieOptions, expires: new Date(token.expiresAt) } as any);
}

async function deleteToken(name: typeof AccessTokenName | typeof RefreshTokenName) {
    const cookie = await cookies();
    cookie.delete(name);
}

export async function getRefreshToken(): Promise<string | undefined> {
    const cookie = await cookies();
    const refreshToken = cookie.get(RefreshTokenName)?.value;
    const decodedToken = refreshToken ? jwtdecode.jwtDecode<{ exp: number }>(refreshToken) : null;
    if (decodedToken) {
        const expiresAt = new Date(decodedToken.exp * 1000);
        if (expiresAt < new Date()) {
            await deleteToken(AccessTokenName);
            await deleteToken(RefreshTokenName);
            return undefined;
        }
    }
    return refreshToken;
}

export async function getAccessToken(): Promise<string | undefined> {
    const cookie = await cookies();
    const accessToken = cookie.get(AccessTokenName)?.value;
    const decodedToken = accessToken ? jwtdecode.jwtDecode<{ exp: number }>(accessToken) : null;
    if (decodedToken) {
        const expiresAt = new Date(decodedToken.exp * 1000);
        if (expiresAt < new Date()) {
            await deleteToken(AccessTokenName);
            return undefined;
        }
    }
    return accessToken;
}

export async function clearTokens() {
    await deleteToken(AccessTokenName);
    await deleteToken(RefreshTokenName);
}

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