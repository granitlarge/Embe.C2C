import { Mutate } from "@/src/shared/apis/api";
import { ApiResponse, FailureReason, } from "@/src/shared/apis/type";
import { Guid } from "@/src/shared/cache";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { Gender, Location} from "@/src/shared/types/domain/value-objects";
import { ReadDto } from "@/src/shared/types/dtos/types";

export async function updateProfile
    (
        userId: Guid,
        alias: string,
        birthDate: string,
        gender?: Gender,
        location?: Location,
        imagesToKeep?: { id: string, order: number }[],
        imagesToAdd?: { url: string, mimeType: string, order: number }[],
        bio?: string
    ): Promise<ApiResponse<ReadDto<User, UserPermission>, FailureReason>> {

    const body = JSON.stringify({ userId, alias, birthDate, gender, location, imagesToKeep, imagesToAdd, bio });
    const response = Mutate<ReadDto<User, UserPermission>, FailureReason>(
        `${process.env.NEXT_PUBLIC_API_URL}/api/user`,
        {
            method: "PUT",
            body: body,
            headers: {
                "Content-Type": "application/json"
            }
        }
    );

    return response;

}