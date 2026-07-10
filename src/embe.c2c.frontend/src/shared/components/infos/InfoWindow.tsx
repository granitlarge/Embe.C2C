"use client";

import { Info } from "@deemlol/next-icons"
import Surface from "../surfaces/Surface"
import { useState } from "react"

export type InfoWindowProps = {
    text: string;
}
export default function InfoWindow({ text }: InfoWindowProps) {

    const [isOpen, setIsOpen] = useState(false)
    return (

        <Surface className="flex flex-row gap-2 max-w-max items-start" variant="tertiary" padding="sm">
            <button onClick={() => setIsOpen(prev => !prev)}>
                <Info />
            </button>
            {
                isOpen && <p className="text-(--primary-fc) text-(length:--primary-fs)">{text}</p>
            }
        </Surface>

    )

}