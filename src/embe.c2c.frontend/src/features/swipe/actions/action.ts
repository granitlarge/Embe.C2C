import { ApiResponse, FailureReason, Read } from "@/src/shared/api";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";

export async function getCandidates(): Promise<ApiResponse<ReadDto<User, UserPermission>[], FailureReason>> {

    const response = await Read<ReadDto<User, UserPermission>[]>(
        `${process.env.API_URL}/api/user/candidates`,
        {
            method: "GET"
        }
    )

    return response;

}