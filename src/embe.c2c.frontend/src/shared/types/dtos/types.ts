export type CreateFile = {
    url: string;
    mimeType: string;
    order: number;
}

export type ReadDto<T_Data, T_Permission> = {
    data: T_Data;
    permissions: T_Permission[];
}