"use client";

import { Info } from "@deemlol/next-icons"
import Surface from "../surfaces/Surface"
import { useState } from "react"

export type InfoWindowProps = {
    text: string;
    show?: boolean
}
export default function InfoWindow({ text, show = false }: InfoWindowProps) {

    const [isOpen, setIsOpen] = useState(show)
    return (

        <Surface className="flex flex-row gap-2 max-w-max items-start" variant="tertiary" padding="sm">
            <button onClick={() => setIsOpen(prev => !prev)}>
                <Info />
            </button>
            {
                isOpen && <p className="text-(--primary-fc) text-(length:--secondary-fs)">{text}</p>
            }
        </Surface>

    )

}