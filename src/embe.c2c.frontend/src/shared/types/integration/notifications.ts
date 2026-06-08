import { MatchingCreatedNotification, MatchingRemovedNotification } from "../domain/aggregates";

export type MatchingCreatedNotificationIntegrationEntity = MatchingCreatedNotification & {
    partnerUserName: string;
    partnerProfileImageUrl: string
}

export type MatchingRemovedNotificationIntegrationEntity = MatchingRemovedNotification & {
    partnerUserName: string;
}