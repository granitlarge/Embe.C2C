import { ResponseCookie } from "next/dist/compiled/@edge-runtime/cookies";

export const RefreshTokenName = "X-Refresh-Token";
export const AccessTokenName = "X-Access-Token";
export const TokenCookieOptions: Partial<ResponseCookie> = { path: "/", httpOnly: true, secure: true, sameSite: "strict" };