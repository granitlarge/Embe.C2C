import LargeModal from "@/src/shared/components/modal/LargeModal";
import { useEffect, useState } from "react";
import useNotificationStore from "../../stores";
import Notification from "../notification/Notification";
import * as api from "../../actions/action";

export type NotificationsModalProps = {
    hidden: boolean;
    closed: () => void;
}

export default function NotificationsModal({ hidden, closed }: NotificationsModalProps) {

    const notifications = useNotificationStore((state) => state.notifications);
    const setNotifications = useNotificationStore((state) => state.setNotifications);
    const [error, setError] = useState<string | null>(null);
    const [hasFetched, setHasFetched] = useState(false);

    useEffect(() => {

        async function fetchNotification() {
            const response = await api.getNotifications();
            if (response.success) {
                setHasFetched(true);
                setError(null);
                setNotifications(response.value!);
            } else {
                setError("failed to fetch notifications");
            }
        }

        if (!hasFetched && !hidden) {
            fetchNotification();
        }

    }, [setNotifications, setError, hasFetched, hidden, setHasFetched]);

    async function remove(notificationId: string) {
        setNotifications(notifications.filter((notification) => notification.id !== notificationId));
    }

    async function markAsUnread(notificationId: string) {
        await api.markAsRead(notificationId, false);
        setNotifications(notifications.map((notification) => {
            if (notification.id === notificationId) {
                return { ...notification, isRead: false };
            }
            return notification;
        }));
    }

    async function markAsRead(notificationId: string) {
        await api.markAsRead(notificationId, true);
        setNotifications(notifications.map((notification) => {
            if (notification.id === notificationId) {
                return { ...notification, isRead: true, };
            }
            return notification;
        }));
    }

    return (
        <LargeModal hidden={hidden} closed={closed} header={"notifications"}>
            {
                error ? <span className="text-(length:--fs-6) error-message">{error}</span> :
                    notifications.length === 0 ? <span className="text-(length:--fs-6)">no notifications</span> :
                        notifications.map((notification) => (
                            <Notification
                                key={notification.id}
                                notification={notification}
                                remove={() => remove(notification.id)}
                                markAsUnread={() => markAsUnread(notification.id)}
                                markAsRead={() => markAsRead(notification.id)}
                            />
                        ))
            }
        </LargeModal>
    )
}