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

export type FileDetails = {
    url: string;
    name: string;
    mimeType: string;
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