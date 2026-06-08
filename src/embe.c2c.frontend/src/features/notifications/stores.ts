import { create } from "zustand";
import { Notification, NotificationType } from "../../shared/types/domain/aggregates";
import { MatchingCreatedNotificationIntegrationEntity } from "@/src/shared/types/integration/notifications";

export type NotificationStore = {
    notifications: Notification[];
    hasUnread: () => boolean;
    setNotifications: (notifications: Notification[]) => void;
};

function sort(notifications: Notification[]) {
    return notifications.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
}

const useNotificationStore = create<NotificationStore>((set, get) => ({

    notifications: [{
        createdAt: new Date().toISOString(),
        id: "1",
        isRead: false,
        type: NotificationType.MatchingCreated,
        matchingId: "1",
        partnerUserName: "test",
        recipientUserId: "1",
        readAt: null,
        partnerProfileImageUrl: "https://avatars.dicebear.com/api/initials/test.svg"
    } as MatchingCreatedNotificationIntegrationEntity],

    hasUnread: () => get().notifications.some((notification) => !notification.isRead),
    setNotifications: (notifications: Notification[]) =>
        set({
            notifications: sort(notifications)
        })
}));

export default useNotificationStore;