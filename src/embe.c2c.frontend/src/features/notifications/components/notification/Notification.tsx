import { getMatchingUrl } from "@/src/shared/route";
import { formatTimeAgo } from "@/src/shared/time";
import { NotificationType, Notification as NotificationTypeDef } from "@/src/shared/types/domain/aggregates";
import { MatchingCreatedNotificationIntegrationEntity } from "@/src/shared/types/integration/notifications";
import { MailOpen, Mail } from "lucide-react";
import { Trash2 } from "@deemlol/next-icons";
import Image from "next/image";
import Link from "next/link";

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

export type NotificationProps = {
    remove: () => void;
    markAsUnread: () => void;
    markAsRead: () => void;
    notification: NotificationTypeDef;
}

export default function Notification({ markAsRead, remove, markAsUnread, notification }: NotificationProps) {

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
            <div className="ml-auto flex flex-col gap-2 mt-auto mb-auto items-end">
                <div className="flex flex-row gap-2">

                    {notification.isRead &&
                        <button title="mark as unread" className="cursor-pointer text-(--surface-font-color)" onClick={markAsUnread}>
                            <MailOpen className="w-5 h-5" />
                        </button>}
                    {!notification.isRead &&
                        <button title="mark as read" className="cursor-pointer text-(--surface-font-color)" onClick={markAsRead}>
                            <Mail className="w-5 h-5" />
                        </button>}
                    <button>

                    </button>
                    <button title="remove" className="cursor-pointer text-(--destructive)" onClick={remove}>
                        <Trash2 className="w-5 h-5" />
                    </button>

                </div>
                <div className="ml-auto text-(--surface-font-color-muted) text-(length:--fs-sm)">{timeAgo}</div>
            </div>
        </div>
    )
}