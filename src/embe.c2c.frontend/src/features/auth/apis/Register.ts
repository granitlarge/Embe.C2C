"use server";

import { SendRequest } from "@/src/shared/api";
import { User } from "@/src/shared/types/aggregates";
import { CreateDto } from "@/src/shared/types/dto";
import { FileDetails } from "@/src/shared/types/entities";
import { Gender, DatingPreferences } from "@/src/shared/types/value-objects";

export type RegisterRequest = {
    email: string,
    password: string,
    birthDate: string,
    gender: Gender,
    datingPreferences: DatingPreferences,
    images: CreateDto<FileDetails>[]
}

export async function register(request: RegisterRequest): Promise<User> {
    throw new Error("Not Implemented");
}