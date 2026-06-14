import { Message } from "./aggregates";
import { FileDetails } from "./value-objects";

export type File = {
    id: string;
    ownerUserId: string;
    fileDetails: FileDetails;
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
    lastMessage?: Message;
    messages?: Message[];
}