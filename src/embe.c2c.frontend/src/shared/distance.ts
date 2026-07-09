export function formatDistance(distanceInKm: number): string {
    if (distanceInKm < 1) {
        return "1 km";
    }
    return `${distanceInKm.toFixed(1)} km`;
}