import { NextRequest, NextResponse } from "next/server";
import { RefreshAccessTokenResponse, Token } from "./shared/security/types";
import * as jwtdecode from "jwt-decode";
import { AccessTokenName, RefreshTokenName, TokenCookieOptions } from "./shared/security/constants";

export async function proxy(request: NextRequest) {
    if (request.nextUrl.pathname.startsWith("/protected")) {
        const accessToken = await getAccessToken(request);
        if (!accessToken) {
            const newAccessToken = await refreshAccessToken(request);
            if (!newAccessToken) {
                const response = NextResponse.redirect(new URL("/public/login", request.url));
                response.cookies.set(AccessTokenName, "", { ...TokenCookieOptions, expires: new Date(0) } as any);
                response.cookies.set(RefreshTokenName, "", { ...TokenCookieOptions, expires: new Date(0) } as any);
                return response;
            } else {
                const response = NextResponse.next();
                response.cookies.set(AccessTokenName, newAccessToken.token, { ...TokenCookieOptions, expires: new Date(newAccessToken.expiresAt) } as any);
                return response;
            }
        }
    }
    return NextResponse.next();
}

async function getAccessToken(request: NextRequest): Promise<string | undefined> {
    const accessToken = request.cookies.get(AccessTokenName)?.value;
    if (accessToken) {
        const decodedToken = jwtdecode.jwtDecode<{ exp: number }>(accessToken);
        const expiresAt = new Date(decodedToken.exp * 1000);
        if (expiresAt < new Date()) {
            return undefined;
        }
    }
    return accessToken;
}

async function refreshAccessToken(request: NextRequest): Promise<Token | undefined> {

    const refreshToken = request.cookies.get(RefreshTokenName)?.value;
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
