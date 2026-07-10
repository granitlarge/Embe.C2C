import { EngagementBoundedness, EngagementFrequency, EngagementMedium, Gender, RelationshipType } from "./types/domain/value-objects";

function typedEntries<T extends object>(
    obj: T
): Array<[keyof T, T[keyof T]]> {
    return Object.entries(obj) as Array<[keyof T, T[keyof T]]>;
}

export function enumerate<T extends Record<string, unknown>>(obj: T) {
    return typedEntries(obj)
        .filter(([key]) => isNaN(Number(String(key))))
        .map(([key, value]) => ({ key, value }));
}

export function parse<T extends Record<string, string | number>>(
    obj: T,
    value: string
): T[keyof T] | undefined {
    const key = Object.keys(obj).find(
        key =>
            isNaN(Number(key)) &&
            key.toLowerCase() === value.toLowerCase()
    );

    return key ? obj[key as keyof T] : undefined;
}

export function formatGender(gender: Gender) {
    switch (gender) {
        case Gender.Male:
            return "Male";
        case Gender.Female:
            return "Female";
        case Gender.TransMale:
            return "Trans Male";
        case Gender.TransFemale:
            return "Trans Female";
        case Gender.Other:
            return "Other";
    }
}

export function formatRelationshipType(relationshipType: RelationshipType) {
    switch (relationshipType) {
        case RelationshipType.Romantic:
            return "Romantic";
        case RelationshipType.Platonic:
            return "Platonic";
        case RelationshipType.Professional:
            return "Professional";
    }
}

export function formatEngagementMedium(engagementMedium: EngagementMedium) {
    switch (engagementMedium) {
        case EngagementMedium.Virtual:
            return "Virtual";
        case EngagementMedium.InPerson:
            return "In-Person";
        case EngagementMedium.Hybrid:
            return "Hybrid";
    }
}

export function formatEngagementBoundedness(engagementBoundedness: EngagementBoundedness) {
    switch (engagementBoundedness) {
        case EngagementBoundedness.OneTime:
            return "One-Time";
        case EngagementBoundedness.Ongoing:
            return "Ongoing";
        case EngagementBoundedness.FixedTerm:
            return "Fixed-Term";
    }
}

export function formatEngagementFrequency(engagementFrequency: EngagementFrequency) {
    switch (engagementFrequency) {
        case EngagementFrequency.Once:
            return "Once";
        case EngagementFrequency.Daily:
            return "Daily";
        case EngagementFrequency.Weekly:
            return "Weekly";
        case EngagementFrequency.Monthly:
            return "Monthly";
        case EngagementFrequency.Quarterly:
            return "Quarterly";
        case EngagementFrequency.Yearly:
            return "Yearly";
    }
}