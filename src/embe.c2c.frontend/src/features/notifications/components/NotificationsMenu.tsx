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

type NotificationProps = {
    remove: () => void;
    notification: NotificationTypeDef;
}

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

function Notification({ remove, notification }: NotificationProps) {

    const timeAgo = formatTimeAgo(notification.createdAt);
    const { title, content, imageUrl, linkUrl } = getNotificationContent(notification);

    const classNames = [
        "flex flex-row gap-0 p-3 w-full border border-(--color-tertiary) rounded-lg",
        linkUrl ? "cursor-pointer" : "",
    ].filter(Boolean).join(" ");

    function getTsx(children: React.ReactNode) {

        if (linkUrl) {
            return <Link className={classNames} href={linkUrl}>{children}</Link>;
        } else {
            return <div className={classNames}>{children}</div>;
        }

    }

    return getTsx
    (
        <>
            <div className="flex flex-row gap-5 items-center">
                {imageUrl && <Image src={imageUrl} alt="notification image" width={0} height={0} className="w-20 h-20 rounded-full" />}
                <div className="flex flex-col gap-0">
                    <span className="text-(--color-secondary) text-(length:--fs-7)">{title}</span>
                    <span className="text-(length:--fs-md) text-(--color-tertiary)">{content}</span>
                </div>
            </div>
            <div className="ml-auto mt-auto text-(--color-tertiary) text-(length:--fs-sm)">{timeAgo}</div>
        </>
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

    async function removeNotification(id: string) {
        setNotifications(notifications.filter((notification) => notification.id !== id));
    }

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
                bg-(--color-quaternary)
                `} closedby="any">
                <h2 className="mr-auto ml-auto text-(--color-tertiary)">notifications</h2>
                {
                    notifications.length === 0 ? <span>no notifications yet</span> :
                        notifications.map((notification) => (
                            <Notification
                                key={notification.id}
                                notification={notification}
                                remove={() => removeNotification(notification.id)}
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