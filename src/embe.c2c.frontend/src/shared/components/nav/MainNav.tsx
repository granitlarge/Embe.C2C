"use client";

import { Users, Heart, MessageCircle, User } from "lucide-react";
import { AuthenticatedUser } from "../../user";
import styles from "./MainNav.module.css";
import Link from "next/link";
import Surface from "../surfaces/Surface";

export type MainNavProps = {
    className?: string;
    user: AuthenticatedUser;
    hasUnseenLikes?: boolean;
    hasUnseenMatches?: boolean;
    hasUnseenMessages?: boolean;
}

export default function MainNav({
    className,
    user,
    hasUnseenLikes,
    hasUnseenMatches,
    hasUnseenMessages
}: MainNavProps) {

    const iconSize = 24;
    const linkClassNames = `flex flex-col items-center justify-center ${styles.link}`;
    const iconClassNames = `relative`;
    const linkTextClassNames = `text-(length:--fs-lg)`;

    const classNames = [className].filter(Boolean).join(" ");

    return (
        <Surface as="nav" className={`${classNames} w-full flex items-center justify-center`} padding="sm">
            <ul className="flex items-center justify-center gap-12">
                <li>
                    <Link href="/protected/swipe" className={linkClassNames}>
                        <Users size={iconSize} className="inline" />
                        <span className={linkTextClassNames}>swipe</span>
                    </Link>
                </li>
                <li>
                    <Link href="/protected/likes" className={linkClassNames}>
                        <div className="relative">
                            <Heart size={iconSize} className={iconClassNames} />
                            {hasUnseenLikes && <span className="absolute top-[1px] -right-[2px] block h-2 w-2 rounded-full bg-(--primary)"></span>}
                        </div>
                        <span className={linkTextClassNames}>likes</span>
                    </Link>
                </li>
                <li>
                    <Link href="/protected/matches" className={linkClassNames}>
                        <div className="relative">
                            <MessageCircle size={iconSize} className={iconClassNames} />
                            {(hasUnseenMatches || hasUnseenMessages) && <span className="absolute top-[2px] -right-[0px] block h-2 w-2 rounded-full bg-(--primary)"></span>}
                        </div>
                        <span className={linkTextClassNames}>matches</span>
                    </Link>
                </li>
                <li>
                    <Link href="/protected/profile" className={linkClassNames}>
                        <User size={iconSize} className={iconClassNames} />
                        <span className={linkTextClassNames}>profile</span>
                    </Link>
                </li>
            </ul>
        </Surface>
    )

}