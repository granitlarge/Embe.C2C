import { Tag } from "../cache";

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

export type ReadRequest = Omit<RequestInit, "method" | "next"> & {
    method: "GET" | "HEAD";
    next?: Omit<RequestInit["next"], "tags"> & {
        tags?: [...Tag[]]
    }
}

export type MutationRequest = Omit<RequestInit, "method"> & {
    method: "POST" | "PUT" | "PATCH" | "DELETE";
    next?: Omit<RequestInit["next"], "tags">
}