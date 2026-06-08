import { MatchingCreatedNotification, MatchingRemovedNotification } from "../domain/aggregates";

export type MatchingCreatedNotificationIntegrationEntity = MatchingCreatedNotification & {
    partnerUserName: string;
}

export type MatchingRemovedNotificationIntegrationEntity = MatchingRemovedNotification & {
    partnerUserName: string;
}
