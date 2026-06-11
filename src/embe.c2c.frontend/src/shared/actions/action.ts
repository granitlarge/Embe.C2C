import { ApiResponse, FailureReason } from "../api";

export async function getHasUnseenLikes(): Promise<ApiResponse<boolean, FailureReason>> {
    throw new Error("Not implemented");
}

export async function getHasUnseenMatches(): Promise<ApiResponse<boolean, FailureReason>> {
    throw new Error("Not implemented");
}

export async function getHasUnseenMessages(): Promise<ApiResponse<boolean, FailureReason>> {
    throw new Error("Not implemented");
}