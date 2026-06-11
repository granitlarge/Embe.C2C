import { Gender, DatingPreferences, FileDetails } from "@/src/shared/types/domain/value-objects";
import { CreateFile } from "@/src/shared/types/dtos/types";

export type RegisterRequest = {
    email: string,
    userName?: string;
    password: string,
    birthDate: string,
    gender: Gender,
    datingPreferences: DatingPreferences,
    files: CreateFile[]
}

export enum RegisterUserFailureReason {
    EmailAlreadyExists = 0,
    DomainError = 1,
    WeakPassword = 2,
    Unknown = 3,
    UnknownError = 4
}