import { Guid } from "../../cache";
import { ReadDto } from "../dtos/types";
import { Message, MessagePermission } from "./aggregates";
import { ImageDetails as ImageDetails } from "./value-objects";

export type Image = {
    id: Guid;
    ownerUserId: Guid;
    imageDetails: ImageDetails;
    markedForDeletionAt: string | null;
    deletedAt: string | null;
    createdAt: string;
}

export type Conversation = {
    id: Guid;
    matchingId: Guid;
    userId1: Guid;
    userId2: Guid;
    lastMessageId: Guid | null;
    messageCount: number;
    updatedAt: string;
    createdAt: string;
    lastMessage?: ReadDto<Message, MessagePermission>;
    messages?: ReadDto<Message, MessagePermission>[];
}