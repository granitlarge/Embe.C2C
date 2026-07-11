"use server";
import type { ApiResponse, FailureReason, ReadRequest, MutationRequest } from "./type";
import { redirect } from "next/navigation";
import { ApiError } from "../api-errors";
import { getAccessToken } from "../security/functions";

async function parseResponse<T>(response: Response): Promise<T | undefined> {

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

async function SendAuthenticatedRequest<T>(request: Request): Promise<T> {

    let accessToken = await getAccessToken();
    if (!accessToken) {
        return redirect("/public/login", "push");
    }

    request.headers.set("Authorization", `Bearer ${accessToken}`);

    const response = await fetch(request);
    if (response.ok) {
        const parsedResponse = await parseResponse<T>(response);
        return parsedResponse!;
    }

    if (response.status === 401) {
        return redirect("/public/login", "push");
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

export async function Read<T>(input: URL | RequestInfo, init: ReadRequest, authenticate?: boolean): Promise<ApiResponse<T, FailureReason>>;
export async function Read<T_Value, T_Error>(input: URL | RequestInfo, init: ReadRequest, authenticate?: boolean): Promise<ApiResponse<T_Value, T_Error>>;
export async function Read<T_Value, T_Error = FailureReason>(input: URL | RequestInfo, init: ReadRequest, authenticate = true): Promise<ApiResponse<T_Value, T_Error>> {
    const request = new Request(input, init);
    if (authenticate) {
        return await SendAuthenticatedRequest<ApiResponse<T_Value, T_Error>>(request);
    }
    return await SendUnauthenticatedRequest<ApiResponse<T_Value, T_Error>>(request);
}

export async function Mutate<T_Value>
    (
        input: URL | RequestInfo,
        init?: MutationRequest,
        authenticate?: boolean
    ): Promise<ApiResponse<T_Value, FailureReason>>;

export async function Mutate<T_Value, T_Error>
    (
        input: URL | RequestInfo,
        init?: MutationRequest,
        authenticate?: boolean

    ): Promise<ApiResponse<T_Value, T_Error>>;

export async function Mutate<T_Value, T_Error = FailureReason>(
    input: URL | RequestInfo,
    init?: MutationRequest,
    authenticate = true
): Promise<ApiResponse<T_Value, T_Error>> {

    const request = new Request(input, init);

    if (authenticate) {
        return await SendAuthenticatedRequest<ApiResponse<T_Value, T_Error>>(request);
    }

    return await SendUnauthenticatedRequest<ApiResponse<T_Value, T_Error>>(request);
}