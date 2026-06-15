"use server";

import { ApiResponse, FailureReason, Mutate, Read } from "@/src/shared/api";
import { Guid, NullGuid } from "@/src/shared/cache";
import { Matching, MatchingPermission, Message, MessagePermission } from "@/src/shared/types/domain/aggregates";
import { CreateMessage, ReadDto } from "@/src/shared/types/dtos/types";
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

export async function getMessages(matchingId: Guid, page: number, size: number): Promise<ApiResponse<ReadDto<Message, MessagePermission>[], FailureReason>> {
    const response = await Read<ReadDto<Message, MessagePermission>[]>
        (
            `${process.env.API_URL}/api/messages?matchingId=${matchingId}&page=${page}&size=${size}`,
            {
                method: "GET",
                next: {
                    tags: [`matching:${matchingId}:message`]
                }
            }
        );
    return response;
}

export async function createMessage(createMessage: CreateMessage): Promise<ApiResponse<ReadDto<Message, MessagePermission>, FailureReason>> {
    const response = await Mutate<ReadDto<Message, MessagePermission>>
        (
            `${process.env.API_URL}/api/messages`,
            {
                method: "POST",
                body: JSON.stringify(createMessage),
                headers: {
                    "Content-Type": "application/json"
                },
            }
        )
    return response;
}

export async function deleteMessage(messageId: Guid): Promise<ApiResponse<void, FailureReason>> {
    const response = await Mutate<void>
        (
            `${process.env.API_URL}/api/messages/${messageId}`,
            {
                method: "DELETE",
            }
        )
    console.log("Delete Message Response:", response);
    return response;
}

export async function updateMessage(messageId: Guid, newContent: string): Promise<ApiResponse<ReadDto<Message, MessagePermission>, FailureReason>> {
    const response = await Mutate<ReadDto<Message, MessagePermission>>
        (
            `${process.env.API_URL}/api/messages`,
            {
                method: "PUT",
                body: JSON.stringify({ messageId, newContent }),
                headers: {
                    "Content-Type": "application/json"
                },
            }
        )
    return response;
}