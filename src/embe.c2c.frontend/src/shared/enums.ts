function typedEntries<T extends object>(
    obj: T
): Array<[keyof T, T[keyof T]]> {
    return Object.entries(obj) as Array<[keyof T, T[keyof T]]>;
}

export function enumerate<T extends Record<string, unknown>>(obj: T) {
    return typedEntries(obj)
        .filter(([key]) => isNaN(Number(String(key))))
        .map(([key, value]) => ({ key, value }));
}

export function parse<T extends Record<string, string | number>>(
    obj: T,
    value: string
): T[keyof T] | undefined {
    const key = Object.keys(obj).find(
        key =>
            isNaN(Number(key)) &&
            key.toLowerCase() === value.toLowerCase()
    );

    return key ? obj[key as keyof T] : undefined;
}