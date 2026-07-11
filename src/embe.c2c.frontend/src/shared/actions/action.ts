import { ApiResponse, FailureReason } from "../apis/type";

export async function getHasUnseenLikes(): Promise<ApiResponse<boolean, FailureReason>> {
    throw new Error("Not implemented");
}

export async function getHasUnseenMatches(): Promise<ApiResponse<boolean, FailureReason>> {
    throw new Error("Not implemented");
}

export async function getHasUnseenMessages(): Promise<ApiResponse<boolean, FailureReason>> {
    throw new Error("Not implemented");
}