import { ApiResponse, FailureReason } from "../apis/type";

export type Token = {
    token: string;
    expiresAt: string;
}

export type RefreshToken = Token & {
    id: string;
}

export type RefreshAccessTokenResponse = ApiResponse<{ accessToken: Token }>;