export type UserBrief = {
    id: string;
    email: string;
    profilePictureUrl: string;
}

export type CreateFile = {
    url: string;
    mimeType: string;
    order: number;
}