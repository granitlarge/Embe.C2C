"use client";

import { Users, Heart, MessageCircle, User } from "lucide-react";
import Surface from "../surfaces/Surface";
import Link from "../Links/Link";

export type MainNavProps = {
    className?: string;
    hasUnseenLikes?: boolean;
    hasUnseenMatches?: boolean;
    hasUnseenMessages?: boolean;
}

export default function MainNav({
    className,
    hasUnseenLikes,
    hasUnseenMatches,
    hasUnseenMessages
}: MainNavProps) {

    const iconSize = 24;
    const linkClassNames = `flex flex-col items-center justify-center text-(--primary-fc) text-(length:--primary-fs) no-underline`;
    const iconClassNames = `relative`;
    const linkTextClassNames = ``;

    const classNames = [className].filter(Boolean).join(" ");

    return (
        <Surface as="nav" className={`${classNames} fs-group-primary py-2`} padding="none" variant="secondary">
            <ul className="flex items-center justify-center gap-5">
                <li>
                    <Link href="/protected/discover" className={linkClassNames}>
                        <Users size={iconSize} className="inline" />
                        <span className={linkTextClassNames}>discover</span>
                    </Link>
                </li>
                <li>
                    <Link href="/protected/search" className={linkClassNames}>
                        <Users size={iconSize} className="inline" />
                        <span className={linkTextClassNames}>search</span>
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
                    <Link href="/protected/me" className={linkClassNames}>
                        <User size={iconSize} className={iconClassNames} />
                        <span className={linkTextClassNames}>me</span>
                    </Link>
                </li>
            </ul>
        </Surface>
    )

}