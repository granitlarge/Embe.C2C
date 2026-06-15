import { File } from "@/src/shared/types/domain/entities";
import { Message } from "../domain/aggregates";
import { Guid } from "../../cache";

export type CreateFile = Pick<File, "fileDetails">;
export type CreateMessage = Pick<Message, "content"> & {
    matchingId: Guid;
};

export type ReadDto<T_Data, T_Permission> = {
    data: T_Data;
    permissions: T_Permission[];
}