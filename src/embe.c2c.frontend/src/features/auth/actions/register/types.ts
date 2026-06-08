import { Gender, DatingPreferences, FileDetails } from "@/src/shared/types/domain/value-objects";

export type RegisterRequest = {
    email: string,
    password: string,
    birthDate: string,
    gender: Gender,
    datingPreferences: DatingPreferences,
    files: FileDetails[]
}

export enum RegisterUserFailureReason {
    EmailAlreadyExists = 0,
    DomainError = 1,
    WeakPassword = 2,
    Unknown = 3,
    UnknownError = 4
}