"use client";
import { Bell } from "@deemlol/next-icons"
import { useEffect, useRef, useState } from "react";
import useNotificationStore from "../stores";
import { NotificationType, Notification as NotificationTypeDef } from "../../../shared/types/domain/aggregates";
import Modal from "@/src/shared/components/modal/Modal";
import styles from "./NotificationsMenu.module.css";
import { MatchingCreatedNotificationIntegrationEntity } from "@/src/shared/types/integration/notifications";
import { formatTimeAgo } from "@/src/shared/time";
import Link from "next/link";
import Image from "next/image";
import Trash from "@/src/shared/components/icons/Trash";
import { MailOpen, Mail } from "lucide-react";
import { getMatchingUrl } from "@/src/shared/route";

type NotificationContent = {
    title: string;
    content: string;
    imageUrl?: string
    linkUrl?: string
}
function getNotificationContent(notification: NotificationTypeDef): NotificationContent {

    let title = "";
    let content = "";
    let imageUrl: string | undefined = undefined;
    let linkUrl: string | undefined = undefined;

    switch (notification.type) {
        case NotificationType.MatchingCreated: {
            const matchingCreatedNotification = notification as MatchingCreatedNotificationIntegrationEntity;
            title = "new match!";
            content = `you matched with ${matchingCreatedNotification.partnerUserName}`;
            imageUrl = matchingCreatedNotification.partnerProfileImageUrl;
            linkUrl = getMatchingUrl(matchingCreatedNotification.matchingId);
            break;
        }
        case NotificationType.MatchingRemoved: {
            const matchingRemovedNotification = notification as MatchingCreatedNotificationIntegrationEntity;
            title = "match removed";
            content = `your match with ${matchingRemovedNotification.partnerUserName} has been removed`;
            break;
        }
    }

    return { title, content, imageUrl, linkUrl };

}

type NotificationProps = {
    remove: () => void;
    markAsUnread: () => void;
    markAsRead: () => void;
    notification: NotificationTypeDef;
}

function Notification({ markAsRead, remove, markAsUnread, notification }: NotificationProps) {

    const timeAgo = formatTimeAgo(notification.createdAt);
    const { title, content, imageUrl, linkUrl } = getNotificationContent(notification);

    const classNames = [
        "flex flex-row gap-0 p-3 w-full border border-(--surface-border-color) rounded-lg bg-(--surface)",
        notification.isRead ? "bg-(--surface)" : "bg-(--surface-light)",
    ].filter(Boolean).join(" ");

    return (
            <div className={classNames}>
                <div className="flex flex-row gap-5 items-center">
                    {imageUrl && <Image src={imageUrl} alt="notification image" width={0} height={0} className="w-12 h-12 rounded-full" />}
                    <div className="flex flex-col gap-0">
                        {
                            linkUrl ? <Link className="text-(length:--fs-7)" href={linkUrl}>{title}</Link> :
                                <span className="text-(--surface-font-color) text-(length:--fs-7)">{title}</span>
                        }
                        <span className="text-(length:--fs-md) text-(--surface-font-color-muted)">{content}</span>
                    </div>
                </div>
                <div className="ml-auto flex flex-col gap-2 mt-auto mb-auto items-center">
                    <div className="flex flex-row gap-2">
                        {notification.isRead &&
                            <button title="mark as unread" className="cursor-pointer text-(--surface-font-color)" onClick={markAsUnread}>
                                <MailOpen className="w-5 h-5" />
                            </button>}
                        {!notification.isRead &&
                            <button title="mark as read" className="cursor-pointer text-(--surface-font-color)" onClick={markAsRead}>
                                <Mail className="w-5 h-5" />
                            </button>}
                        <Trash title="remove" className="w-5 h-5 text-(--destructive) cursor-pointer" onClick={remove} />
                    </div>
                    <div className="ml-auto text-(--surface-font-color-muted) text-(length:--fs-sm)">{timeAgo}</div>
                </div>
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
        setNotifications(notifications.map((notification) => {
            if (notification.id === notificationId) {
                return { ...notification, isRead: false };
            }
            return notification;
        }));
    }

    async function markAsRead(notificationId: string) {
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
                `} closedby="any">
                <h2 className="mr-auto ml-auto text-(--surface-font-color)">notifications</h2>
                {
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
                    {hasUnread() && <span className="absolute top-0 -right-1 block h-2 w-2 rounded-full bg-(--primary)"></span>}
                </div>
            </button>
            <NotificationsModal hidden={hidden} closed={() => setHidden(true)} />
        </div>
    );

}