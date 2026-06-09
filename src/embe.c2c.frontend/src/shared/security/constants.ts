import { ResponseCookie } from "next/dist/compiled/@edge-runtime/cookies";

export const RefreshTokenName = "rt";
export const AccessTokenName = "at";
export const TokenCookieOptions: Partial<ResponseCookie> = { path: "/", httpOnly: true, secure: true, sameSite: "strict" };