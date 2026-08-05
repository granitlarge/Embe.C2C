"use client";

import { Users, Heart, MessageCircle, User } from "lucide-react";
import Surface from "../surfaces/Surface";
import Link from "../Links/Link";
import { Routes } from "../../routes";
import { useApplicationStore } from "../../stores/provider";
import { NotificationType } from "../../types/domain/aggregates";
import { usePathname } from "next/navigation";
import { useEffect } from "react";
import { markAsRead } from "../../actions/notifications/action";

export type MainNavProps = {
    className?: string;
}

export default function MainNav({
    className
}: MainNavProps) {

    const pathname = usePathname();
    const notifications = useApplicationStore(s => s.notifications);
    const setNotifications = useApplicationStore(s => s.setNotifications);

    const hasUnseenMatches = notifications.some(n => n.data.type === NotificationType.MatchingCreated && n.data.isRead === false);
    const hasUnseenMessages = notifications.some(n => n.data.type === NotificationType.MessageCreated && n.data.isRead === false);
    const hasUnseenLikes = notifications.some(n => n.data.type === NotificationType.PositivelyJudged && n.data.isRead === false)

    useEffect(() => {


        async function markUnreadNotificationsAsRead() {

            const markUnreadMatchCreatedNotificationsAsReadPromise = markUnreadMatchCreatedNotificationsAsRead();
            const markUnreadLikesAsReadPromise = markUnreadLikesAsRead();

            await Promise.all([markUnreadLikesAsReadPromise, markUnreadMatchCreatedNotificationsAsReadPromise]);

            async function markUnreadLikesAsRead() {

                if (pathname !== Routes.protected.likes) {
                    return
                }

                const unseenLikesNotifications = notifications.filter(n => n.data.type === NotificationType.PositivelyJudged && n.data.isRead === false);
                if (unseenLikesNotifications.length === 0) {
                    return;
                }

                try {

                    await Promise.all(unseenLikesNotifications.map(umn => markAsRead(umn.data.id!, true)));
                    setNotifications(prev => prev.map(n => {
                        const umn = unseenLikesNotifications.find(umn => umn.data.id === n.data.id);
                        if (!umn) return n;
                        return {
                            ...umn,
                            data: {
                                ...umn.data,
                                isRead: true,
                                readAt: new Date().toISOString(),
                            }
                        }
                    }));

                } catch (e) {

                    console.error(e);

                }

            }

            async function markUnreadMatchCreatedNotificationsAsRead() {

                if (pathname !== Routes.protected.matches) {
                    return
                }

                const unseenMatchNotifications = notifications.filter(n => n.data.type === NotificationType.MatchingCreated && n.data.isRead === false);
                if (unseenMatchNotifications.length === 0) {
                    return;
                }

                try {

                    await Promise.all(unseenMatchNotifications.map(umn => markAsRead(umn.data.id!, true)));
                    setNotifications(prev => prev.map(n => {
                        const umn = unseenMatchNotifications.find(umn => umn.data.id === n.data.id);
                        if (!umn) return n;
                        return {
                            ...umn,
                            data: {
                                ...umn.data,
                                isRead: true,
                                readAt: new Date().toISOString(),
                            }
                        }
                    }));

                } catch (e) {

                    console.error(e);

                }

            }

        }

        markUnreadNotificationsAsRead();

    }, [pathname, notifications, setNotifications])

    const iconSize = 24;
    const linkClassNames = `flex flex-col items-center justify-center text-(--primary-fc) text-(length:--primary-fs) no-underline`;
    const iconClassNames = `relative`;
    const linkTextClassNames = ``;

    const classNames = [className].filter(Boolean).join(" ");

    return (
        <Surface as="nav" className={`${classNames} fs-group-primary py-2`} padding="none" variant="secondary">
            <ul className="flex items-center justify-center gap-5">
                <li>
                    <Link href={Routes.protected.discover} className={linkClassNames}>
                        <Users size={iconSize} className={iconClassNames} />
                        <span className={`${linkTextClassNames} ${pathname === Routes.protected.discover ? "font-bold" : ""}`}>discover</span>
                    </Link>
                </li>
                <li>
                    <Link href={Routes.protected.search} className={linkClassNames}>
                        <Users size={iconSize} className={iconClassNames} />
                        <span className={`${linkTextClassNames} ${pathname === Routes.protected.search ? "font-bold" : ""}`}>search</span>
                    </Link>
                </li>
                <li>
                    <Link href={Routes.protected.likes} className={linkClassNames}>
                        <div className="relative">
                            <Heart size={iconSize} className={iconClassNames} />
                            {hasUnseenLikes && <span className="absolute top-[1px] -right-[2px] block h-2 w-2 rounded-full bg-(--important-fc)"></span>}
                        </div>
                        <span className={`${linkTextClassNames} ${pathname === Routes.protected.likes ? "font-bold" : ""}`}>likes</span>
                    </Link>
                </li>
                <li>
                    <Link href={Routes.protected.matches} className={linkClassNames}>
                        <div className="relative">
                            <MessageCircle size={iconSize} className={iconClassNames} />
                            {(hasUnseenMatches || hasUnseenMessages) && <span className="absolute -top-[2px] -right-[4px] block h-2 w-2 rounded-full bg-(--important-fc)"></span>}
                        </div>
                        <span className={`${linkTextClassNames} ${pathname === Routes.protected.matches ? "font-bold" : ""}`}>matches</span>
                    </Link>
                </li>
                <li>
                    <Link href={Routes.protected.me} className={linkClassNames}>
                        <User size={iconSize} className={iconClassNames} />
                        <span className={`${linkTextClassNames} ${pathname === Routes.protected.me ? "font-bold" : ""}`}>me</span>
                    </Link>
                </li>
            </ul>
        </Surface>
    )

}