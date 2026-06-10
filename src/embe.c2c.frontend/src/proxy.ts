import { NextRequest, NextResponse } from "next/server";
import { getAccessToken, refreshAccessToken } from "./shared/security/functions";
import { AccessTokenName, RefreshTokenName, TokenCookieOptions } from "./shared/security/constants";

export async function proxy(request: NextRequest) {
    if (request.nextUrl.pathname.startsWith("/protected")) {
        const accessToken = await getAccessToken();
        if (!accessToken) {
            const newAccessToken = await refreshAccessToken();
            if (!newAccessToken) {
                const response = NextResponse.redirect(new URL("/public/login", request.url));
                response.cookies.set(AccessTokenName, "", { ...TokenCookieOptions, expires: new Date(0) });
                response.cookies.set(RefreshTokenName, "", { ...TokenCookieOptions, expires: new Date(0) });
                return response;
            } else {
                const response = NextResponse.next();
                response.cookies.set(AccessTokenName, newAccessToken.token, { ...TokenCookieOptions, expires: new Date(newAccessToken.expiresAt) });
                return response;
            }
        }
    }
    return NextResponse.next();
}