"use server";

import { ResponseCookie } from "next/dist/compiled/@edge-runtime/cookies";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { ApiError } from "./api-errors";

const refreshTokenCookieName = "X-Refresh-Token";
const accessTokenCookieName = "X-Access-Token";
const cookieOptions: Partial<ResponseCookie> = { path: "/", httpOnly: true, secure: true, sameSite: "strict" };

async function getRefreshToken(): Promise<string | undefined> {
    const cookie = await cookies();
    const refreshToken = cookie.get(refreshTokenCookieName)?.value;
    return refreshToken;
}

async function getAccessToken() {
    const cookie = await cookies();
    const accessToken = cookie.get(accessTokenCookieName)?.value;
    return accessToken;
}

type Token = {
    token: string;
    expiresAt: string;
}

type RefreshAccessTokenResponse = {
    accessToken: Token;
}

async function refreshAccessToken(): Promise<Token | undefined> {
    const refreshToken = await getRefreshToken();
    if (!refreshToken) {
        return undefined;
    }

    const response = await fetch("/api/auth/refresh", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${refreshToken}`
        }
    });

    if (!response.ok) {
        return undefined;
    }

    const responseBody = await response.json();
    const { accessToken } = JSON.parse(responseBody) as RefreshAccessTokenResponse;
    return accessToken;
}

async function setCookie(name: string, token: Token) {
    const cookie = await cookies();
    cookie.set(name, token.token, { ...cookieOptions, expires: new Date(token.expiresAt) });
}

async function deleteCookie(name: string) {
    const cookie = await cookies();
    cookie.delete(name);
}

async function SendAuthenticatedRequest<T>(request: Request): Promise<T> {

    let accessToken = await getAccessToken();
    if (!accessToken) {
        const newAccessToken = await refreshAccessToken();
        if (!newAccessToken) {
            return redirect("/login", "push");
        }
        await setCookie(accessTokenCookieName, newAccessToken);
        accessToken = newAccessToken.token;
    }

    request.headers.set("Authorization", `Bearer ${accessToken}`);

    const response = await fetch(request);
    if (response.ok) {
        return await parseResponse(response);
    }

    if (response.status === 401) {
        const newAccessToken = await refreshAccessToken();
        if (!newAccessToken) {
            return redirect("/login", "push");
        }

        await setCookie(accessTokenCookieName, newAccessToken);
        request.headers.set("Authorization", `Bearer ${newAccessToken.token}`);
        const retryResponse = await fetch(request);
        if (retryResponse.ok) {
            return await parseResponse(retryResponse);
        }

        if (retryResponse.status === 401) {
            await deleteCookie(accessTokenCookieName);
            await deleteCookie(refreshTokenCookieName);
            return redirect("/login", "push");
        }
    }

    throw new ApiError(response);

    async function parseResponse<T>(response: Response): Promise<T> {
        const responseBody = await response.json();;
        return JSON.parse(responseBody) as T;
    }
}

async function SendUnauthenticatedRequest<T>(request: Request): Promise<T> {
    const response = await fetch(request);
    if (response.ok) {
        const responseBody = await response.json();
        return JSON.parse(responseBody) as T;
    }
    throw new ApiError(response);
}

export async function SendRequest<T>(request: Request, authenticated = true): Promise<T> {
    if (authenticated) {
        return await SendAuthenticatedRequest<T>(request);
    }
    return await SendUnauthenticatedRequest<T>(request);
}