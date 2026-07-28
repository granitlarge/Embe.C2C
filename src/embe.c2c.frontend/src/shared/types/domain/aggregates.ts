import { Guid } from "../../cache";
import { ReadDto } from "../dtos/types";
import { Image } from "./entities";
import { DatingPreferences, Engagement, Gender, Location, Money, RelationshipType, TransactionReason, TransactionType } from "./value-objects";

export type User = {
    id: Guid;
    email?: string;
    alias?: string;
    birthDate?: string;
    age?: number;
    gender?: Gender;
    datingPreferences?: DatingPreferences;
    location?: Location;
    images?: Image[];
    createdAt?: string;
    updatedAt?: string;
    distanceKmToQueryingUser?: number;
    bio?: string;
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

export type Matching = {
    id: Guid;
    userId1: Guid;
    userId2: Guid;
    userId1SearchProfileId?: Guid;
    userId2SearchProfileId?: Guid;
    createdAt?: string;
    user1?: ReadDto<User, UserPermission>;
    user2?: ReadDto<User, UserPermission>;
    user1SearchProfile?: ReadDto<SearchProfile, SearchProfilePermission>;
    user2SearchProfile?: ReadDto<SearchProfile, SearchProfilePermission>;
    lastMessage?: ReadDto<Message, MessagePermission>;
    messages?: ReadDto<Message, MessagePermission>[];
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
    MessageCreated = 1,
    PositivelyJudged = 2
}

export type Notification = {
    type: NotificationType;
    id?: Guid;
    recipientUserId?: Guid;
    isRead?: boolean;
    readAt?: string;
    createdAt?: string;
}

export enum NotificationPermission {
    View = 1,
    Delete = 2,
    MarkAsRead = 3,
}

export type MatchingCreatedNotification = Notification & {
    matchingId: Guid;
    recipientUserId: Guid;
    partnerUserId: Guid;
}

export type MessageCreatedNotification = Notification & {
    messageId: Guid;
}

export type PositivelyJudgedNotification = Notification & {
    candidateId: Guid;
    userId: Guid;
    candidateUserId: Guid;
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

export type SearchProfile = {
    id: Guid;
    userId: Guid;
    active?: boolean;
    name?: string;
    description?: string;
    relationshipType?: RelationshipType;
    engagement?: Engagement;
    genders?: Gender[];
    ageRangeMin?: number;
    ageRangeMax?: number;
    maximumDistanceKm?: number;
    createdAt?: string;
    updatedAt?: string;
}

export enum SearchProfilePermission {
    View = 0,
    Modify = 1
}

export type Candidate = {
    id: Guid,
    userId: Guid,
    candidateUserId: Guid,
    userSearchProfileId: Guid,
    candidateSearchProfileId: Guid,
    createdAt?: string,
    updatedAt?: string;
    judgement?: boolean,
    user?: ReadDto<User, UserPermission>,
    candidate?: ReadDto<User, UserPermission>,
    userSearchProfile: ReadDto<SearchProfile, SearchProfilePermission>,
    candidateSearchProfile: ReadDto<SearchProfile, SearchProfilePermission>
}

export enum CandidatePermission {
    View = 1,
    Judge = 2
}