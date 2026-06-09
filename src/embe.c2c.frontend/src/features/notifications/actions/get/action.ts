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