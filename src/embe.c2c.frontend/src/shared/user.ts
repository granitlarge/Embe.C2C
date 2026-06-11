import { Guid } from "./cache";
import { getRefreshToken } from "./security/functions";
import { jwtDecode } from "jwt-decode";

export type AuthenticatedUser = {
    identityUserId: Guid;
    userId: Guid;
};

export async function getAuthenticatedUser(): Promise<AuthenticatedUser | undefined> {
    const refreshToken = await getRefreshToken();
    if (!refreshToken) {
        return undefined;
    }
    const decoded = jwtDecode(refreshToken) as { sub: string; userId: string; };
    return {
        identityUserId: decoded.sub as Guid,
        userId: decoded.userId as Guid,
    };
}