"use server";

import { ApiResponse, FailureReason, SendRequest } from "@/src/shared/api";

export async function getNotifications(): Promise<ApiResponse<Notification[], FailureReason>> {
    const response = await SendRequest<Notification[]>
        (
            new Request(`${process.env.API_URL}/api/notifications`, {
                method: "GET",
            })
        );

    return response;
}