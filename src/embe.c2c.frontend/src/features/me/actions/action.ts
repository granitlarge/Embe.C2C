import { ApiResponse, FailureReason, Mutate } from "@/src/shared/api";
import { Image } from "@/src/shared/types/domain/entities";

export async function updateImages
    (
        filesToKeep: { id: string, order: number }[],
        filesToAdd: { url: string, mimeType: string, order: number }[]
    ): Promise<ApiResponse<Image[], FailureReason>> {
    const response = Mutate<Image[], FailureReason>(
        `${process.env.NEXT_PUBLIC_API_URL}/api/user/me/images`,
        {
            method: "POST",
            body: JSON.stringify({ filesToKeep, filesToAdd }),
            headers: {
                "Content-Type": "application/json"
            }
        }
    );
    return response;
}