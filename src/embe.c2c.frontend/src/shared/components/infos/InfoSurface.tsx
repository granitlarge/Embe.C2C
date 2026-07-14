"use client";

import { Info } from "@deemlol/next-icons"
import Surface from "../surfaces/Surface"
import { useState } from "react"

export type InfoSurfaceProps = {
    show?: boolean
    children: React.ReactNode;
    className?: string;
}
export default function InfoSurface({ className, children, show = false }: InfoSurfaceProps) {

    const [isOpen, setIsOpen] = useState(show)
    const classNames = [className].filter(Boolean).join(" ");
    return (

        <Surface className={`flex flex-row gap-2 items-start ${classNames}`} variant="tertiary" padding="sm">
            <button onClick={() => setIsOpen(prev => !prev)}>
                <Info />
            </button>
            {
                isOpen && children
            }
        </Surface>

    )

}