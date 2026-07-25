import { Tag } from "../cache";

export type Error = {
    code: string;
    description: string;
    type: ErrorType;
}

export enum ErrorType {
    Failure = 0,
    Unexpected = 1,
    Validation = 2,
    Conflict = 3,
    NotFound = 4,
    Unauthorized = 5,
    Forbidden = 6
}

export type ApiResponse<T_Value> = {
    success: boolean;
    value?: T_Value;
    errors?: Error[];
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