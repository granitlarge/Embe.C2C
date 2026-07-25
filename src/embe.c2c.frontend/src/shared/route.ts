import { Routes } from "./routes";

export function getMatchingUrl(matchingId: string): string {
    return Routes.protected.match(matchingId);
}