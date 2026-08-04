"use client";

import React from "react"
import Surface from "../surfaces/Surface"
import { ChevronDown } from "lucide-react";

export type CollapsibleSectionProps = {
    title: string
    children: React.ReactNode
    headingLevel?: 2 | 3 | 4 | 5 | 6
}
export default function CollapsibleSection({ headingLevel = 2, title, children }: CollapsibleSectionProps) {

    const [isOpen, setIsOpen] = React.useState(false)

    const heading = React.createElement(`h${headingLevel}`, { className: "text-(--primary-fs)" }, title);
    return (
        <Surface className="flex flex-col gap-3" variant="secondary" padding="sm">
            <div className="flex justify-between items-center cursor-pointer" onClick={() => setIsOpen(!isOpen)}>
                {heading}
                <ChevronDown className={`transition-transform duration-300 ${isOpen ? "rotate-180" : ""}`} />
            </div>
            {isOpen && children}
        </Surface>
    )

}