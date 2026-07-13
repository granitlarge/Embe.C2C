import { Gender, Location } from "@/src/shared/types/domain/value-objects";

export type RegisterRequest = {
    email: string,
    alias?: string;
    password: string,
    birthDate: string,
    location: Location;
    gender: Gender;
}

export enum RegisterUserFailureReason {
    EmailAlreadyExists = 0,
    DomainError = 1,
    WeakPassword = 2,
    Unknown = 3,
    UnknownError = 4
}