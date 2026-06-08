"use client";

import { Bell } from "@deemlol/next-icons"
import { useEffect, useRef, useState } from "react";
import useNotificationStore from "../stores";
import { Notification as NotificationTypeDef } from "../../../shared/types/domain/aggregates";
import Modal from "@/src/shared/components/modal/Modal";
import styles from "./NotificationsMenu.module.css";

type NotificationProps = {
    remove: () => void;
    notification: NotificationTypeDef;
}

function Notification({ remove, notification }: NotificationProps) {

    let loaded = false;
    let title: string = "";
    let content: string = "";

    return (

        <div>

        </div>

    )

}

type NotificationsModalProps = {
    hidden: boolean;
    closed: () => void;
}
function NotificationsModal({ hidden, closed }: NotificationsModalProps) {

    const notifications = useNotificationStore((state) => state.notifications);
    const setNotifications = useNotificationStore((state) => state.setNotifications);
    const dialog = useRef<HTMLDialogElement | null>(null);

    function close() {
        closed();
        dialog.current?.close();
    }

    useEffect(() => {
        if (hidden) {
            dialog.current?.close();
        } else {
            if (notifications.some((notification) => !notification.isRead)) {
                setNotifications(notifications.map((notification) => ({ ...notification, isRead: true })));
            }
            dialog.current?.showModal();
        }
        dialog.current?.addEventListener("close", close);
        return () => {
            dialog.current?.removeEventListener("close", close);
        }
    }, [hidden, closed, setNotifications, notifications]);

    const classNames = [
        hidden ? "hidden" : "",
    ].filter(Boolean).join(" ");

    return (
        <div className={`${classNames} fixed top-0 left-0 w-[100dvw] h-[100dvh] flex flex-col items-center justify-center backdrop-blur-xs`}>
            <Modal ref={dialog} className={`
                flex flex-col items-center
                ${styles.modal}
                z-1000 
                p-5 
                flex 
                flex-col 
                w-[70%] h-[70%] 
                m-auto 
                rounded-lg 
                bg-(--color-quaternary-light)
                `} closedby="any">
                <h2 className="mr-auto ml-auto text-(--color-tertiary)">notifications</h2>
                {
                    notifications.length === 0 ? <span>no notifications yet</span> :
                        notifications.map((notification) => (
                            <Notification
                                key={notification.id}
                                notification={notification}
                                remove={() => setNotifications(notifications.filter((n) => n.id !== notification.id))}
                            />
                        ))
                }
            </Modal>

        </div>
    )
}

export type NotificationsMenuProps = {
    className?: string;
    notifications?: NotificationTypeDef[];
};
export default function NotificationsMenu({ className, notifications: initialNotifications }: NotificationsMenuProps) {

    const notifications = useNotificationStore((state) => state.notifications);
    const setNotifications = useNotificationStore((state) => state.setNotifications);
    const hasUnread = useNotificationStore((state) => state.hasUnread);

    useEffect(() => {
        setNotifications(initialNotifications || notifications);
    }, [initialNotifications, setNotifications]);

    const [hidden, setHidden] = useState(true);

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <div className="flex flex-col ml-auto">
            <button
                className={`${classNames} cursor-pointer`}
                onClick={() => { setHidden(prev => !prev) }}
            >
                <div className="relative">
                    <Bell size={36} className="relative" />
                    {hasUnread() && <span className="absolute top-0 -right-1 block h-2 w-2 rounded-full bg-(--color-secondary)"></span>}
                </div>
            </button>
            <NotificationsModal hidden={hidden} closed={() => setHidden(true)} />
        </div>
    );

}