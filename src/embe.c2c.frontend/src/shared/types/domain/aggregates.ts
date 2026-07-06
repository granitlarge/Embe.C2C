import { Guid } from "../../cache";
import { ReadDto } from "../dtos/types";
import { Conversation, Image } from "./entities";
import { DatingPreferences, Gender, Location, Money, TransactionReason, TransactionType } from "./value-objects";

export type User = {
    id: Guid;
    email?: string;
    userName?: string;
    birthDate?: string;
    age?: number;
    gender?: Gender;
    datingPreferences?: DatingPreferences;
    location?: Location;
    profilePicture?: Image;
    images?: Image[];
    createdAt?: string;
    updatedAt?: string;
}

export enum UserPermission {
    View = 0,
    Update = 1,
    Delete = 2
}

export type Account = {
    id: Guid;
    userId: Guid;
    balance: Money;
    isOpen: boolean;
}

export type Blocking = {
    id: Guid;
    blockerUserId: Guid;
    blockedUserId: Guid;
    blockedAt: string;
}

export type Contact = {
    id: Guid;
    userId1: Guid;
    userId2: Guid;
    createdAt: string;
}

export type ContactRequest = {
    id: Guid;
    requestorUserId: Guid;
    recipientUserId: Guid;
    isAccepted: boolean | null;
    respondedAt: string | null;
    requestedAt: string;
}

export type Judgement = {
    id: Guid;
    judgeUserId: Guid;
    judgeeUserId: Guid;
    isPositive?: boolean;
    editedAt?: string;
    createdAt?: string;
    judge?: ReadDto<User, UserPermission>;
}

export enum JudgementPermission {
    View = 0
}

export type Matching = {
    id: Guid;
    userId1: Guid;
    userId2: Guid;
    conversation?: Conversation;
    createdAt?: string;
    user1?: ReadDto<User, UserPermission>;
    user2?: ReadDto<User, UserPermission>;
}

export enum MatchingPermission {
    View = 0,
    Unmatch = 1
}

export type Message = {
    id: Guid;
    conversationId: Guid;
    replyToMessageId?: Guid;
    authorUserId: Guid;
    content?: string;
    seenAt?: string;
    createdAt?: string;
    editedAt?: string;
    isReply?: boolean;
    replyToMessage?: ReadDto<Message, MessagePermission>;
}

export enum MessagePermission {
    View = 0,
    Edit = 1,
    Delete = 2,
    Report = 3,
    Reply = 4,
    MarkAsSeen = 5
}

export enum NotificationType {
    MatchingCreated = 0,
    MatchingRemoved = 1,
    PositiveJudgementReceived = 2,
}

export type Notification = {
    type: NotificationType;
    id: Guid;
    recipientUserId: Guid;
    isRead: boolean;
    readAt: string | null;
    createdAt: string;
}

export type MatchingCreatedNotification = Notification & {
    matchingId: Guid;
    partnerUserId: Guid;
    partnerUserName: string;
    partnerProfileImageUrl: string;
}

export type MatchingRemovedNotification = Notification & {
    matchingId: Guid;
    partnerUserId: Guid;
    partnerUserName: string;
    partnerProfileImageUrl: string;
}

export type Transaction = {
    id: Guid;
    accountId: Guid;
    amount: Money;
    type: TransactionType;
    reason: TransactionReason;
    transactionDate: string;
    note: string | null;
    createdAt: string;
}