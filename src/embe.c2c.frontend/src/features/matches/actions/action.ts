"use server";

import { ApiResponse, FailureReason, Read } from "@/src/shared/api";
import { NullGuid } from "@/src/shared/cache";
import { Matching, MatchingPermission, Message, MessagePermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { getAuthenticatedUser } from "@/src/shared/user";

export async function getMatchings(page: number, size: number): Promise<ApiResponse<ReadDto<Matching, MatchingPermission>[], FailureReason>> {
    const user = await getAuthenticatedUser();
    const response = await Read<ReadDto<Matching, MatchingPermission>[]>
        (
            `${process.env.API_URL}/api/matching?page=${page}&size=${size}`,
            {
                method: "GET",
                next: {
                    tags: [`user:${user?.userId || NullGuid}:matching`]
                }
            }
        );
    return response;
}

export async function getMatching(matchId: string): Promise<ApiResponse<ReadDto<Matching, MatchingPermission>, FailureReason>> {
    const user = await getAuthenticatedUser();
    const response = await Read<ReadDto<Matching, MatchingPermission>>
        (
            `${process.env.API_URL}/api/matching/${matchId}`,
            {
                method: "GET",
                next: {
                    tags: [`user:${user?.userId || NullGuid}:matching`]
                }
            }
        );
    return response;
}

export async function getMessages(conversationId: string, page: number, size: number): Promise<ApiResponse<ReadDto<Message, MessagePermission>[], FailureReason>> {
    throw new Error("Not implemented");
}