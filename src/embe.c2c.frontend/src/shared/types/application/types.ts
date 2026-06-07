export type AccessToken = {
    token: string;
    expiresAt: string;
}

export type RefreshToken = {
    id: string;
    token: string;
    expiresAt: string;
}

export type Credentials = {
    accessToken: AccessToken;
    refreshToken: RefreshToken;
}