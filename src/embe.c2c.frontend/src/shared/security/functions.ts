"use server";

import { cookies } from "next/headers";
import { AccessTokenName, RefreshTokenName, TokenCookieOptions } from "./constants";

export type Token = {
    token: string;
    expiresAt: string;
}

export type RefreshToken = Token & {
    id: string;
}

export async function saveToken(name: string, token: Token) {
    const cookie = await cookies();
    cookie.set(name, token.token, { ...TokenCookieOptions, expires: new Date(token.expiresAt) });
}

export async function deleteToken(name: typeof AccessTokenName | typeof RefreshTokenName) {
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