import { Read } from "@/src/shared/apis/api";
import { ApiResponse, FailureReason,  } from "@/src/shared/apis/type";
import { NullGuid } from "@/src/shared/cache";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { getAuthenticatedUser } from "@/src/shared/user";

export async function getCurrentUser(): Promise<ApiResponse<ReadDto<User, UserPermission>, FailureReason>> {

    const currentUser = await getAuthenticatedUser();
    const response = await Read<ReadDto<User, UserPermission>, FailureReason>(`${process.env.API_URL}/api/user/me`, {
        method: "GET",
        next: {
            tags: [`user:${currentUser?.userId || NullGuid}`]
        }
    });

    return response;

}