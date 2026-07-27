import { create } from "zustand";
import { Notification } from "../../shared/types/domain/aggregates";

export type NotificationStore = {
    notifications: Notification[];
    hasUnread: boolean;
    setNotifications: (notifications: Notification[]) => void;
    setHasUnread: (hasUnread: boolean) => void;
};

function sort(notifications: Notification[]) {
    return notifications.sort((a, b) => new Date(b.createdAt!).getTime() - new Date(a.createdAt!).getTime());
}

const useNotificationStore = create<NotificationStore>((set) => ({
    notifications: [],
    hasUnread: false,
    setNotifications: (notifications: Notification[]) =>
        set({
            notifications: sort(notifications),
            hasUnread: notifications.some(notification => !notification.isRead)
        }),
    setHasUnread: (hasUnread: boolean) =>
        set({
            hasUnread
        })
}));

export default useNotificationStore;