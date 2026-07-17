"use client";

import { Bell } from "@deemlol/next-icons"
import { useEffect, useState } from "react";
import useNotificationStore from "../../stores";
import NotificationsModal from "../notifications-modal/NotificationsModal";
import * as api from "../../actions/action";
import Button from "@/src/shared/components/buttons/Button";

export type NotificationsMenuProps = {
    className?: string;
};

export default function NotificationsMenu({ className }: NotificationsMenuProps) {

    const setHasUnread = useNotificationStore((state) => state.setHasUnread);
    const hasUnread = useNotificationStore((state) => state.hasUnread);

    useEffect(() => {
        async function fetchHasUnread() {
            const result = await api.hasUnread()
            if (result.value) {
                setHasUnread(result.value);
            }
        }
        fetchHasUnread();
    }, [setHasUnread]);

    const [hidden, setHidden] = useState(true);

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <div className="flex flex-col ml-auto">
            <Button
                className={`${classNames} cursor-pointer`}
                onClick={() => { setHidden(prev => !prev) }}
            >
                <div className="relative flex flex-col items-center justify-center">
                    <Bell size={36} className="relative" />
                    <span className="text-(length:--fs-lg)">notifications</span>
                    {hasUnread && <span className="absolute top-0 -right-1 block h-2 w-2 rounded-full bg-(--primary)"></span>}
                </div>
            </Button>
            <NotificationsModal hidden={hidden} closed={() => setHidden(true)} />
        </div>
    );

}