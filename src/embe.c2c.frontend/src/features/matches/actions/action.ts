import { ApiResponse, FailureReason, Read } from "@/src/shared/api";
import { NullGuid } from "@/src/shared/cache";
import { Matching } from "@/src/shared/types/domain/aggregates";
import { getAuthenticatedUser } from "@/src/shared/user";

export async function getMatches(): Promise<ApiResponse<Matching[], FailureReason>> {
    const user = await getAuthenticatedUser();
    const response = await Read<Matching[]>
        (
            `${process.env.API_URL}/api/matching`,
            {
                method: "GET",
                next: {
                    tags: [`user:${user?.userId || NullGuid}:matching`]
                }
            }
        );
    return response;
}