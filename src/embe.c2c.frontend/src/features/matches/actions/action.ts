"use server";

import { Read, Mutate } from "@/src/shared/apis/api";
import { ApiResponse, FailureReason } from "@/src/shared/apis/type";
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

export async function getMatching(matchId: Guid): Promise<ApiResponse<ReadDto<Matching, MatchingPermission>, FailureReason>> {
    const user = await getAuthenticatedUser();
    const response = await Read<ReadDto<Matching, MatchingPermission>>
        (
            `${process.env.API_URL}/api/matching/${matchId}`,
            {
                method: "GET",
                next: {
                    tags: [`user:${user?.userId || NullGuid}:matching`, `matching:${matchId}`]
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

export async function markMessageAsSeen(...messageIds: Guid[]): Promise<ApiResponse<void, FailureReason>> {
    if (messageIds.length === 0) {
        return { success: true };
    }
    const response = await Mutate<void>
        (
            `${process.env.API_URL}/api/messages/mark-as-seen`,
            {
                method: "POST",
                body: JSON.stringify({ messageIds }),
                headers: {
                    "Content-Type": "application/json"
                },
            }
        )
    return response;
}

export async function getMessage(messageId: Guid): Promise<ApiResponse<ReadDto<Message, MessagePermission>, FailureReason>> {
    const response = await Read<ReadDto<Message, MessagePermission>>
        (
            `${process.env.API_URL}/api/messages/${messageId}`,
            {
                method: "GET",
                next: {
                    tags: [`message:${messageId}`]
                }
            }
        );
    return response;
}

export async function unmatch(matchId: Guid): Promise<ApiResponse<void, FailureReason>> {
    const response = await Mutate<void>(
        `${process.env.API_URL}/api/matching/${matchId}`,
        {
            method: "DELETE",
        }
    )
    return response;
}