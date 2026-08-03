"use client";

import React from "react"
import Surface from "../surfaces/Surface"
import { ChevronDown } from "lucide-react";

export type CollapsibleSectionProps = {
    title: string
    children: React.ReactNode
}
export default function CollapsibleSection({ title, children }: CollapsibleSectionProps) {

    const [isOpen, setIsOpen] = React.useState(false)

    return (
        <Surface className="flex flex-col gap-3" variant="secondary" padding="sm">
            <div className="flex justify-between items-center cursor-pointer" onClick={() => setIsOpen(!isOpen)}>
                <h2>{title}</h2>
                <ChevronDown className={`transition-transform duration-300 ${isOpen ? "rotate-180" : ""}`} />
            </div>
            {isOpen && children}
        </Surface>
    )

}