"use server";

import { ApiResponse, FailureReason, SendRequest } from "@/src/shared/api";
import { Notification } from "@/src/shared/types/domain/aggregates";

export async function getNotifications(): Promise<ApiResponse<Notification[], FailureReason>> {
    const response = await SendRequest<Notification[]>
        (
            new Request(`${process.env.API_URL}/api/notification`, {
                method: "GET",
            })
        );

    return response;
}

export async function markAsRead(notificationId: string, isRead: boolean): Promise<ApiResponse<void, FailureReason>> {
    const response = await SendRequest<void, FailureReason>
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
    const response = await SendRequest<boolean, FailureReason>
        (
            new Request(`${process.env.API_URL}/api/notification/has-unread`, {
                method: "GET",
            })
        );

    return response;
}