"use client";

import { useState } from "react";
import { User } from "@deemlol/next-icons";

export type UserMenuProps = {
    className?: string;
}

export default function UserMenu({ className }: UserMenuProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const [hidden, setHidden] = useState(true);

    const menuClassNames =
        [
            hidden ? "hidden" : "",
        ].filter(Boolean).join(" ");

    return (
        <div className={`relative ${classNames}`}>
            <button onClick={() => setHidden(prev => !prev)} className="cursor-pointer flex flex-col items-center justify-center">
                <User size={36} />
                <span className="text-(length:--fs-lg)">user</span>
            </button>
            <ul className={`${menuClassNames} absolute right-0 bg-(--surface) text-(--surface-font-color)`}>
                <li>Profile</li>
                <li>Settings</li>
                <li>Logout</li>
            </ul>
        </div>
    )

}