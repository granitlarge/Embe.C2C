import Modal from "@/src/shared/components/modal/Modal";
import { useRef, useEffect, useState } from "react";
import useNotificationStore from "../../stores";
import Notification from "../notification/Notification";
import styles from "./NotificationsModal.module.css";
import * as api from "../../actions/action";

export type NotificationsModalProps = {
    hidden: boolean;
    closed: () => void;
}

export default function NotificationsModal({ hidden, closed }: NotificationsModalProps) {

    const notifications = useNotificationStore((state) => state.notifications);
    const setNotifications = useNotificationStore((state) => state.setNotifications);
    const dialog = useRef<HTMLDialogElement | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [hasFetched, setHasFetched] = useState(false);

    function close() {
        closed();
        dialog.current?.close();
    }

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

    useEffect(() => {

        if (hidden) {
            dialog.current?.close();
        } else {
            dialog.current?.showModal();
        }

        dialog.current?.addEventListener("close", close);

        return () => {
            dialog.current?.removeEventListener("close", close);
        }

    }, [hidden, closed, setNotifications, notifications]);

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

    const classNames = [
        hidden ? "hidden" : "",
    ].filter(Boolean).join(" ");

    return (
        <div className={`${classNames} fixed top-0 left-0 w-[100dvw] h-[100dvh] flex flex-col items-center justify-center backdrop-blur-xs`}>
            <Modal ref={dialog} className={`
                flex flex-col items-center gap-5
                ${styles.modal}
                z-1000 
                p-5 
                flex 
                flex-col 
                w-[70%] h-[70%] 
                m-auto 
                rounded-lg 
                bg-(--surface)
                scrollbar-gutter-stable
                `} closedby="any">
                <h2 className="mr-auto ml-auto text-(--surface-font-color)">notifications</h2>
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
            </Modal>
        </div>
    )
}