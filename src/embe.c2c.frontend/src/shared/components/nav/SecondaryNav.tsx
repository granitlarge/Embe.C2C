"use client";
import Surface from "../surfaces/Surface";
import { Settings } from "@deemlol/next-icons"

export type SecondaryNavProps = {
    className?: string;
}

export default function SecondaryNav({ className }: SecondaryNavProps) {

    return (
        <Surface as="nav" className={`rounded-md p-2 flex flex-row gap-2 ${className}`}>
            <Settings className="ml-auto" />
        </Surface>
    );
}