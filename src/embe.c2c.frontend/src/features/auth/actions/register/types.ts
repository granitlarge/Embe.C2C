import { Gender, Location } from "@/src/shared/types/domain/value-objects";

export type RegisterRequest = {
    email: string,
    alias?: string;
    password: string,
    birthDate: string,
    gender: Gender;
    location?: Location;
    images: ImageWriteDto[];
}

export enum RegisterUserFailureReason {
    EmailAlreadyExists = 0,
    DomainError = 1,
    WeakPassword = 2,
    Unknown = 3,
    UnknownError = 4
}

export type ImageWriteDto = {
    base64EncodedImageData: string,
    mimeType: string,
    order: number,
    cropOffsetX: number,
    cropOffsetY: number,
    width: number,
    height: number,
}