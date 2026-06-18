"use server";

import { ApiResponse, FailureReason, Mutate, Read } from "@/src/shared/api";
import { NullGuid } from "@/src/shared/cache";
import { Matching, MatchingPermission, User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { getAuthenticatedUser } from "@/src/shared/user";

export async function getCandidates(): Promise<ApiResponse<ReadDto<User, UserPermission>[], FailureReason>> {

    const user = await getAuthenticatedUser();
    const response = await Read<ReadDto<User, UserPermission>[]>(
        `${process.env.API_URL}/api/user/candidates`,
        {
            method: "GET",
            next: {
                tags: [`user:${user?.userId || NullGuid}:candidate`]
            }
        }
    )

    return response;

}

export async function judge(candidateId: string, isPositive: boolean): Promise<ApiResponse<ReadDto<Matching | undefined, MatchingPermission>, FailureReason>> {
    const response = await Mutate<ReadDto<Matching | undefined, MatchingPermission>>(
        `${process.env.API_URL}/api/judgement`,
        {
            method: "POST",
            body: JSON.stringify({ judgeeUserId: candidateId, isPositive }),
            headers: {
                "Content-Type": "application/json"
            }
        }
    )
    return response;
}