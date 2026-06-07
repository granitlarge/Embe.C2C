import { Gender, DatingPreferences, FileDetails } from "@/src/shared/types/domain/value-objects";

export type RegisterRequest = {
    email: string,
    password: string,
    birthDate: string,
    gender: Gender,
    datingPreferences: DatingPreferences,
    files: FileDetails[]
}