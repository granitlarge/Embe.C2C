import { getRefreshToken } from "./security/functions";
import {jwtDecode} from "jwt-decode";

export type AuthenticatedUser = {
    id: string;
    userId: string;
};

export async function getAuthenticatedUser() : Promise<AuthenticatedUser | undefined> {
    const refreshToken = await getRefreshToken();
    if (!refreshToken) {
        return undefined;
    }
    const decoded = jwtDecode(refreshToken) as { sub: string; userId: string; };
    return {
        id: decoded.sub,
        userId: decoded.userId,
    };
}