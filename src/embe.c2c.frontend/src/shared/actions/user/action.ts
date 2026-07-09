import { ApiResponse, FailureReason, Read } from "../../api";
import { Guid } from "../../cache";
import { User, UserPermission } from "../../types/domain/aggregates";
import { ReadDto } from "../../types/dtos/types";

export async function getProfile(profileId: Guid): Promise<ApiResponse<ReadDto<User, UserPermission>, FailureReason>> {
    const response = await Read<ReadDto<User, UserPermission>>
        (
            `${process.env.API_URL}/api/user/${profileId}`,
            {
                method: "GET",
                next: {
                    tags: [`user:${profileId}`]
                }
            }
        );
    return response;
}