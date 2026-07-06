import { Image } from "@/src/shared/types/domain/entities";
import { Message } from "../domain/aggregates";
import { Guid } from "../../cache";
import { ImageDetails } from "../domain/value-objects";

export type CreateFile = Omit<ImageDetails, "name">;
export type CreateMessage = Pick<Message, "content" | "replyToMessageId"> & {
    matchingId: Guid;
};

export type ReadDto<T_Data, T_Permission> = {
    data: T_Data;
    permissions: T_Permission[];
}