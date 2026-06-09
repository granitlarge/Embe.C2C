"use client";

import { Bell } from "@deemlol/next-icons"
import { useEffect, useState } from "react";
import useNotificationStore from "../../stores";
import NotificationsModal from "../notifications-modal/NotificationsModal";

export type NotificationsMenuProps = {
    className?: string;
    hasUnread?: boolean;
};

export default function NotificationsMenu({ className, hasUnread: initialHasUnread }: NotificationsMenuProps) {

    const setHasUnread = useNotificationStore((state) => state.setHasUnread);
    const hasUnread = useNotificationStore((state) => state.hasUnread);

    useEffect(() => {
        setHasUnread(initialHasUnread ?? false);
    }, [initialHasUnread, setHasUnread]);

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
                    {hasUnread && <span className="absolute top-0 -right-1 block h-2 w-2 rounded-full bg-(--primary)"></span>}
                </div>
            </button>
            <NotificationsModal hidden={hidden} closed={() => setHidden(true)} />
        </div>
    );

}