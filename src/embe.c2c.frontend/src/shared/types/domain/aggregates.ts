import { Conversation, File } from "./entities";
import { DatingPreferences, Gender, Location, Money, TransactionReason, TransactionType } from "./value-objects";

export type User = {
    id: string;
    email: string;
    userName: string;
    birthDate: string;
    gender: Gender;
    datingPreferences: DatingPreferences;
    location: Location;
    files: File[];
    createdAt: string;
    updatedAt: string;
}

export type Account = {
    id: string;
    userId: string;
    balance: Money;
    isOpen: boolean;
}

export type Blocking = {
    id: string;
    blockerUserId: string;
    blockedUserId: string;
    blockedAt: string;
}

export type Contact = {
    id: string;
    userId1: string;
    userId2: string;
    createdAt: string;
}

export type ContactRequest = {
    id: string;
    requestorUserId: string;
    recipientUserId: string;
    isAccepted: boolean | null;
    respondedAt: string | null;
    requestedAt: string;
}

export type Judgement = {
    id: string;
    judgeUserId: string;
    judgeeUserId: string;
    isPositive: boolean;
    editedAt: string;
    createdAt: string;
}

export type Matching = {
    id: string;
    userId1: string;
    userId2: string;
    conversation: Conversation;
    createdAt: string;
}

export type Message = {
    id: string;
    conversationId: string;
    replyToMessageId: string | null;
    authorUserId: string;
    content: string;
    seenAt: string | null;
    createdAt: string;
    editedAt: string;
}

export enum NotificationType {
    MatchingCreated = 0,
    MatchingRemoved = 1,
}

export type Notification = {
    type: NotificationType;
    id: string;
    recipientUserId: string;
    isRead: boolean;
    readAt: string | null;
    createdAt: string;
}

export type MatchingCreatedNotification = Notification & {
    matchingId: string;
    partnerUserId: string;
    partnerUserName: string;
    partnerProfileImageUrl: string;
}

export type MatchingRemovedNotification = Notification & {
    matchingId: string;
    partnerUserId: string;
    partnerUserName: string;
    partnerProfileImageUrl: string;
}

export type Transaction = {
    id: string;
    accountId: string;
    amount: Money;
    type: TransactionType;
    reason: TransactionReason;
    transactionDate: string;
    note: string | null;
    createdAt: string;
}