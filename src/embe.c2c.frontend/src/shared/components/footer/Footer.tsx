"use client";

import { AuthenticatedUser, getAuthenticatedUser } from "../../user";
import Link from "next/link";
import { User, Users, Heart, MessageCircle } from "@deemlol/next-icons";
import Surface from "../surfaces/Surface";
import styles from "./Footer.module.css";
import useNotificationStore from "@/src/features/notifications/stores";
import { NotificationType } from "../../types/domain/aggregates";
import { useEffect, useState } from "react";

export type FooterProps = {
    className?: string;
};
export default function Footer({ className }: FooterProps) {

    const notifications = useNotificationStore(state => state.notifications);
    const hasUnread = useNotificationStore(state => state.hasUnread);
    const unseenLikes = hasUnread && notifications.some(n => n.type === NotificationType.PositiveJudgementReceived && !n.readAt);
    const unseenMatches = hasUnread && notifications.some(n => n.type === NotificationType.MatchingCreated && !n.readAt);
    const [user, setUser] = useState<AuthenticatedUser | undefined>(undefined);

    useEffect(() => {
        getAuthenticatedUser()
            .then(setUser);
    }, []);

    const iconSize = 24;
    const linkClassNames = `flex flex-col items-center justify-center ${styles.link}`;
    const iconClassNames = `relative`;
    const linkTextClassNames = `text-(length:--fs-lg)`;

    return (
        <Surface as="footer" padding="sm" className={className}>
            {user &&
                <nav className="flex items-center justify-center gap-12">
                    <Link href="/dating" className={linkClassNames}>
                        <Users size={iconSize} className="inline" />
                        <span className={linkTextClassNames}>dating</span>
                    </Link>
                    <Link href="/likes" className={linkClassNames}>
                        <div className="relative">
                            <Heart size={iconSize} className={iconClassNames} />
                            {unseenLikes && <span className="absolute top-[1px] -right-[2px] block h-2 w-2 rounded-full bg-(--primary)"></span>}
                        </div>
                        <span className={linkTextClassNames}>likes</span>
                    </Link>
                    <Link href="/matches" className={linkClassNames}>
                        <div className="relative">
                            <MessageCircle size={iconSize} className={iconClassNames} />
                            {unseenMatches && <span className="absolute top-[2px] -right-[0px] block h-2 w-2 rounded-full bg-(--primary)"></span>}
                        </div>
                        <span className={linkTextClassNames}>matches</span>
                    </Link>
                    <Link href="/profile" className={linkClassNames}>
                        <User size={iconSize} className={iconClassNames} />
                        <span className={linkTextClassNames}>profile</span>
                    </Link>
                </nav>
            }
        </Surface>
    )

}