import { Read } from "../../apis/api";
import { ApiResponse, FailureReason } from "../../apis/type";
import { Guid, NullGuid } from "../../cache";
import { User, UserPermission } from "../../types/domain/aggregates";
import { ReadDto } from "../../types/dtos/types";
import { getAuthenticatedUser } from "../../user";

export async function getUser(userId: Guid): Promise<ApiResponse<ReadDto<User, UserPermission>>> {
    const response = await Read<ReadDto<User, UserPermission>>
        (
            `${process.env.API_URL}/api/user/${userId}`,
            {
                method: "GET",
                next: {
                    tags: [`user:${userId}`]
                }
            }
        );
    return response;
}

export async function getHasSearchProfile(): Promise<ApiResponse<boolean>> {
    const authenticatedUser = await getAuthenticatedUser();
    const response = await Read<boolean>
        (
            `${process.env.API_URL}/api/user/has-search-profile`,
            {
                method: "GET",
                next: {
                    tags: [`user:${authenticatedUser?.userId || NullGuid}:search-profile`]
                }
            }
        );
    return response;
}