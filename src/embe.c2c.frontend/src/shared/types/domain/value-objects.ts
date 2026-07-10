export type Distance = {
    value: number;
    unit: LengthUnit;
}

export type Location = {
    latitude: number;
    longitude: number;
}

export type DatingPreferences = {
    interestedInGenders: Gender[];
    ageRangeMin: number;
    ageRangeMax: number;
    maximumDistance: Distance;
}

export type ImageDetails = {
    url: string;
    name: string;
    mimeType: string;
    order: number;
}

export type Currency = {
    code: string;
    name: string;
    symbol: string;
}

export type Money = {
    amount: number;
    currency: Currency;
}

export enum LengthUnit {
    Kilometers = 0,
    Miles = 1,
}

export enum Gender {
    Male = 0,
    Female = 1,
    TransMale = 2,
    TransFemale = 3,
    Other = 4,
}

export enum TransactionType {
    Deposit = 0,
    Withdrawal = 1,
}

export enum TransactionReason {
    Purchase = 1,
    Sale = 2,
    Refund = 3,
    Withdrawal = 4,
    Deposit = 5,
}

export enum EngagementMedium {
    Virtual = 0,
    InPerson = 1,
    Hybrid = 2,
}

export enum EngagementBoundedness {
    OneTime = 0,
    Ongoing = 1,
    FixedTerm = 2,
}

export enum EngagementFrequency {
    Once = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    Yearly = 5,
}

export enum RelationshipType {
    Romantic = 0,
    Platonic = 1,
    Professional = 2
}

export type Engagement = {
    medium: EngagementMedium;
    boundedness: EngagementBoundedness;
    frequency: EngagementFrequency;
    startDate: string;
    endDate: string;
}