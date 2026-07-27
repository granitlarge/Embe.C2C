"use server";

import { Read } from "../../apis/api";
import { ApiResponse } from "../../apis/type";
import { Guid } from "../../cache";
import { Candidate, CandidatePermission } from "../../types/domain/aggregates";
import { ReadDto } from "../../types/dtos/types";

export async function getCandidate(candidateId: Guid): Promise<ApiResponse<ReadDto<Candidate, CandidatePermission>>> {
    const response = await Read<ReadDto<Candidate, CandidatePermission>>(
        `${process.env.API_URL}/api/candidate/${candidateId}`,
        {
            method: "GET",
        }
    )

    return response;
}