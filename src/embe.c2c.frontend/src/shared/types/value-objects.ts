export type Distance = {
    value: number;
}

export type Location = {
    latitude: number;
    longitude: number;
}

export type DatingPreferences = {
    interestedInGenders: Gender[];
    ageRangeMin: number;
    ageRangeMax: number;
    maximumDistance: Distance
}

export enum LengthUnit {
    Kilometer = 0,
    Mile = 1,
}

export enum Gender {
    Male = 0,
    Female = 1,
    TransMale = 2,
    TransFemale = 3,
    Other = 4,
}