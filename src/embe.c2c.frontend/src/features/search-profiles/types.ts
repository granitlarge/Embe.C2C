import { EngagementBoundedness, EngagementFrequency, EngagementMedium, Gender, RelationshipType } from "@/src/shared/types/domain/value-objects";

export type SearchProfileWriteDto = {
    id?: string;
    name: string;
    description: string;
    relationshipType: RelationshipType;
    engagement: EngagementWriteDto;
    genders: Gender[];
    ageRangeMin?: number;
    ageRangeMax?: number;
    maximumDistance?: number;
}
export type EngagementWriteDto = {
    medium: EngagementMedium;
    boundedness: EngagementBoundedness;
    frequency: EngagementFrequency;
    startDate?: string;
    endDate?: string;
}