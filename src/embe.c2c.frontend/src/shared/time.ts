import { Range } from "./types/range";

export function formatTimeAgo(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (diffInSeconds < 60) {
        return `${diffInSeconds} second${diffInSeconds !== 1 ? "s" : ""} ago`;
    } else if (diffInSeconds < 3600) {
        const minutes = Math.floor(diffInSeconds / 60);
        return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
    } else if (diffInSeconds < 86400) {
        const hours = Math.floor(diffInSeconds / 3600);
        return `${hours} hour${hours > 1 ? "s" : ""} ago`;
    } else {
        const days = Math.floor(diffInSeconds / 86400);
        return `${days} day${days > 1 ? "s" : ""} ago`;
    }
}

export function getValidBirthdateRange(minimumAge: number, maximumAge: number): Range<string> {
    const today = new Date();
    const minDate = new Date(today.getFullYear() - maximumAge, today.getMonth(), today.getDate());
    const maxDate = new Date(today.getFullYear() - minimumAge, today.getMonth(), today.getDate());
    return {
        lower: minDate.toISOString().split('T')[0],
        upper: maxDate.toISOString().split('T')[0]
    };
}

export function calculateAge(birthdate: string): number {
    const birthDate = new Date(birthdate);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDifference = today.getMonth() - birthDate.getMonth();
    if (monthDifference < 0 || (monthDifference === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }
    return age;
}