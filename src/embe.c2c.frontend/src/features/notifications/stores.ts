import { create } from "zustand";
import { Notification } from "../../shared/types/domain/aggregates";

export type NotificationStore = {
    notifications: Notification[];
    hasUnread: () => boolean;
    setNotifications: (notifications: Notification[]) => void;
};

function sort(notifications: Notification[]) {
    return notifications.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
}

const useNotificationStore = create<NotificationStore>((set, get) => ({
    notifications: [],
    hasUnread: () => get().notifications.some((notification) => !notification.isRead),
    setNotifications: (notifications: Notification[]) =>
        set({
            notifications: sort(notifications)
        })
}));

export default useNotificationStore;