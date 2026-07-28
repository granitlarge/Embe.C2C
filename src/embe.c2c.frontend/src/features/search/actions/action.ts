"use server";

import { ApiResponse, FailureReason } from "@/src/shared/apis/type";
import { Guid, NullGuid } from "@/src/shared/cache";
import { Candidate, CandidatePermission, Matching, MatchingPermission, SearchProfile, SearchProfilePermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { getAuthenticatedUser } from "@/src/shared/user";
import { Read, Mutate } from "@/src/shared/apis/api";

export async function generateCandidates(): Promise<ApiResponse<ReadDto<Candidate, CandidatePermission>[]>> {

    const user = await getAuthenticatedUser();
    const response = await Read<ReadDto<Candidate, CandidatePermission>[]>
    (
        `${process.env.API_URL}/api/candidate`,
        {
            method: "GET",
            next: {
                tags: [`user:${user?.userId || NullGuid}:candidate`]
            }
        }
    )

    return response;

}

export async function judge(candidateId: Guid, isPositive: boolean): Promise<ApiResponse<ReadDto<Matching, MatchingPermission> | undefined>> {
    const response = await Mutate<ReadDto<Matching, MatchingPermission> | undefined>(
        `${process.env.API_URL}/api/candidate/judge`,
        {
            method: "POST",
            body: JSON.stringify({ candidateId, isPositive }),
            headers: {
                "Content-Type": "application/json"
            }
        }
    )
    return response;
}

export async function getAllSearchProfiles(page: number, pageSize: number): Promise<ApiResponse<ReadDto<SearchProfile, SearchProfilePermission>[]>> {

    const response = await Read<ReadDto<SearchProfile, SearchProfilePermission>[]>
        (
            `${process.env.API_URL}/api/search-profile?page=${page}&pageSize=${pageSize}`,
            {
                method: "GET"
            }
        )
    return response;

}