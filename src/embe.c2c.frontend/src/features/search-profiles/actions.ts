"use server";

import { ApiResponse, FailureReason } from "@/src/shared/apis/type";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { SearchProfileWriteDto } from "./types";
import { SearchProfile, SearchProfilePermission } from "@/src/shared/types/domain/aggregates";
import { getAuthenticatedUser } from "@/src/shared/user";
import { Guid, NullGuid } from "@/src/shared/cache";
import { Mutate, Read } from "@/src/shared/apis/api";

export async function createSearchProfile(body: SearchProfileWriteDto): Promise<ApiResponse<ReadDto<SearchProfile, SearchProfilePermission>, FailureReason>> {
    const response = await Mutate<ReadDto<SearchProfile, SearchProfilePermission>, FailureReason>(
        `${process.env.API_URL}/api/search-profile`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
        }
    )
    return response;
}

export async function updateSearchProfile(body: SearchProfileWriteDto): Promise<ApiResponse<ReadDto<SearchProfile, SearchProfilePermission>, FailureReason>> {
    const response = await Mutate<ReadDto<SearchProfile, SearchProfilePermission>, FailureReason>(
        `${process.env.API_URL}/api/search-profile`,
        {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
        }
    )
    return response;
}

export async function getSearchProfile(id: Guid): Promise<ApiResponse<ReadDto<SearchProfile, SearchProfilePermission>, FailureReason>> {
    const user = await getAuthenticatedUser();
    const response = await Read<ReadDto<SearchProfile, SearchProfilePermission>, FailureReason>(`${process.env.API_URL}/api/search-profile/${id}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        next: {
            tags: [`user:${user?.userId || NullGuid}:search-profile`, `search-profile:${id}`],
        }
    });
    return response;
}

export async function deleteSearchProfile(id: Guid): Promise<ApiResponse<void, FailureReason>> {
    const response = await Mutate<void, FailureReason>(`${process.env.API_URL}/api/search-profile/${id}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
    });
    return response;
}