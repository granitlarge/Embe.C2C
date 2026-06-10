"use server";

import { ApiResponse, FailureReason, Mutate, Read } from "@/src/shared/api";
import { Tag } from "@/src/shared/cache";
import { Notification } from "@/src/shared/types/domain/aggregates";
import { getAuthenticatedUser } from "@/src/shared/user";

async function getUserNotificationTag(): Promise<Tag> {
    const user = await getAuthenticatedUser();
    const tag: Tag = `user:${user?.userId || crypto.randomUUID()}:notification`;
    return tag;
}

export async function getNotifications(): Promise<ApiResponse<Notification[], FailureReason>> {
    const response = await Read<Notification[]>
    (
        `${process.env.API_URL}/api/notification`,
        {
            method: "GET",
            next: {
                tags: [await getUserNotificationTag()]
            }
        }
    );

    return response;
}

export async function markAsRead(notificationId: string, isRead: boolean): Promise<ApiResponse<void, FailureReason>> {
    const response = await Mutate<void, FailureReason>
        (
            new Request(`${process.env.API_URL}/api/notification/mark-as-read`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ notificationId, isRead })
            })
        );

    return response;
}

export async function hasUnread(): Promise<ApiResponse<boolean, FailureReason>> {
    const response = await Read<boolean, FailureReason>
        (
            `${process.env.API_URL}/api/notification/has-unread`,
            {
                method: "GET",
                next: {
                    tags: [await getUserNotificationTag()]
                }
            }
        );

    return response;
}