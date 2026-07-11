import { ApiResponse, FailureReason, Read } from "../../api";
import { Guid, NullGuid } from "../../cache";
import { User, UserPermission } from "../../types/domain/aggregates";
import { ReadDto } from "../../types/dtos/types";
import { getAuthenticatedUser } from "../../user";

export async function getUser(userId: Guid): Promise<ApiResponse<ReadDto<User, UserPermission>, FailureReason>> {
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

export async function getHasSearchProfile(): Promise<ApiResponse<boolean, FailureReason>> {
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