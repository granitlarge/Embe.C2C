"use server";

import { Read, Mutate } from "@/src/shared/apis/api";
import { ApiResponse, FailureReason, } from "@/src/shared/apis/type";
import { Guid, NullGuid, Tag } from "@/src/shared/cache";
import { Notification } from "@/src/shared/types/domain/aggregates";
import { getAuthenticatedUser } from "@/src/shared/user";
import { ReadDto } from "../../types/dtos/types";

async function getUserNotificationTag(): Promise<Tag> {
    const user = await getAuthenticatedUser();
    const tag: Tag = `user:${user?.userId || NullGuid}:notification`;
    return tag;
}

export async function getNotifications(): Promise<ApiResponse<ReadDto<Notification, NotificationPermission>[]>> {
    const response = await Read<ReadDto<Notification, NotificationPermission>[]>
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

export async function markAsRead(notificationId: string, isRead: boolean): Promise<ApiResponse<void>> {
    const response = await Mutate<void>
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

export async function hasUnread(): Promise<ApiResponse<boolean>> {
    const response = await Read<boolean>
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

export async function getNotification(id: Guid): Promise<ApiResponse<ReadDto<Notification, NotificationPermission>>> {
    const response = await Read<ReadDto<Notification, NotificationPermission>>
        (
            `${process.env.API_URL}/api/notification/${id}`,
            {
                method: "GET"
            }
        )
    return response;
}