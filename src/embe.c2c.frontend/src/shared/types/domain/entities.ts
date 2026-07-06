import { ReadDto } from "../dtos/types";
import { Message, MessagePermission } from "./aggregates";
import { ImageDetails as ImageDetails } from "./value-objects";

export type Image = {
    id: string;
    ownerUserId: string;
    imageDetails: ImageDetails;
    markedForDeletionAt: string | null;
    deletedAt: string | null;
    createdAt: string;
}

export type Conversation = {
    id: string;
    matchingId: string;
    userId1: string;
    userId2: string;
    lastMessageId: string | null;
    messageCount: number;
    updatedAt: string;
    createdAt: string;
    lastMessage?: ReadDto<Message, MessagePermission>;
    messages?: ReadDto<Message, MessagePermission>[];
}