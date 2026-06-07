"use server";

import { redirect } from "next/navigation";
import { ApiError } from "./api-errors";
import { AccessTokenName, RefreshTokenName } from "./security/constants";
import { deleteToken, getAccessToken, getRefreshToken, saveToken, Token } from "./security/functions";

type RefreshAccessTokenResponse = {
    accessToken: Token;
}

async function parseResponse<T>(response: Response) : Promise<T | undefined>
{
    try {
        const contentType = response.headers.get("Content-Type");
        if (contentType && contentType.includes("application/json")) {
            const responseBody = await response.json();
            return responseBody as T;
        }
    } catch (error) {

    }
    return undefined;
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
        },
        body: JSON.stringify({ refreshToken })
    });

    if (!response.ok) {
        return undefined;
    }

    const responseBody = await response.json();
    const { accessToken } = responseBody as RefreshAccessTokenResponse;
    return accessToken;
}

async function SendAuthenticatedRequest<T>(request: Request): Promise<T> {

    let accessToken = await getAccessToken();
    if (!accessToken) {
        const newAccessToken = await refreshAccessToken();
        if (!newAccessToken) {
            return redirect("/login", "push");
        }
        await saveToken(AccessTokenName, newAccessToken);
        accessToken = newAccessToken.token;
    }

    request.headers.set("Authorization", `Bearer ${accessToken}`);

    const response = await fetch(request);
    if (response.ok) {
        const parsedResponse = await parseResponse<T>(response);
        return parsedResponse!;
    }

    if (response.status === 401) {
        const newAccessToken = await refreshAccessToken();
        if (!newAccessToken) {
            return redirect("/login", "push");
        }

        await saveToken(AccessTokenName, newAccessToken);
        request.headers.set("Authorization", `Bearer ${newAccessToken.token}`);
        const retryResponse = await fetch(request);
        if (retryResponse.ok) {
            const parsedRetryResponse = await parseResponse<T>(retryResponse);
            return parsedRetryResponse!;
        }

        if (retryResponse.status === 401) {
            await deleteToken(AccessTokenName);
            await deleteToken(RefreshTokenName);
            return redirect("/login", "push");
        }
    }

    const parsedErrorResponse = await parseResponse<T>(response);
    if (parsedErrorResponse) {
        return parsedErrorResponse;
    }

    const error = await ApiError.fromResponse(response);
    throw error;
}

async function SendUnauthenticatedRequest<T>(request: Request): Promise<T> {
    const response = await fetch(request);
    const parsedResponse = await parseResponse<T>(response);
    if (parsedResponse) {
        return parsedResponse;
    }
    const error = await ApiError.fromResponse(response);
    throw error;
}

export type ApiResponse<T_Value, T_Error> = {
    success: boolean;
    value?: T_Value;
    reason?: T_Error;
    message?: string;
}

export enum FailureReason {
    NotFound = 0,
    Forbidden = 1,
    DomainError = 2,
    Unknown = 3
}

export async function SendRequest<T_Value>(
    request: Request,
    authenticate?: boolean
): Promise<ApiResponse<T_Value, FailureReason>>;

export async function SendRequest<T_Value, T_Error>(
    request: Request,
    authenticate?: boolean
): Promise<ApiResponse<T_Value, T_Error>>;

export async function SendRequest<T_Value, T_Error = FailureReason>(
    request: Request,
    authenticate = true
): Promise<ApiResponse<T_Value, T_Error>> {
    if (authenticate) {
        return await SendAuthenticatedRequest<ApiResponse<T_Value, T_Error>>(request);
    }
    return await SendUnauthenticatedRequest<ApiResponse<T_Value, T_Error>>(request);
}