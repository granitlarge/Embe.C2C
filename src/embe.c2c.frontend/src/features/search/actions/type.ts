import { Guid } from "@/src/shared/cache";
import { SearchProfile, SearchProfilePermission, User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";

export type GeneratedCandidate = {
    id: Guid;
    candidate: ReadDto<User, UserPermission>;
    userSearchProfileId: string;
    candidateSearchProfile: ReadDto<SearchProfile, SearchProfilePermission>;
};

export type GenerateCandidatesResponse = GeneratedCandidate[];