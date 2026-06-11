import React from "react";

export type Padding = "none" | "sm" | "md" | "lg";
export type Color = "default" | "primary";

export type SurfaceProps<T extends React.ElementType = "div"> = {
    className?: string;
    children: React.ReactNode;
    padding?: Padding;
    as?: T;
    transparent?: boolean;
    backgroundColor?: Color;
    color?: Color;
} & React.ComponentPropsWithoutRef<T>;

export default function Surface<T extends React.ElementType = "div">({
    className,
    children,
    padding = "md",
    as,
    transparent,
    backgroundColor = "default",
    color = "default",
    ...props
}: SurfaceProps<T>) {
    const Component = as || "div";

    const classNames = [
        className,

        padding === "none" ? "p-0" :
        padding === "sm" ? "p-2" :
        padding === "md" ? "p-4" :
        "p-6",

        transparent ? "bg-transparent" : 
        backgroundColor === "default" ? "bg-(--surface)" :
        backgroundColor === "primary" ? "bg-(--primary)" :
        "bg-(--surface)",
    ]
        .filter(Boolean)
        .join(" ");

    return (
        <Component
            {...props}
            className={`${classNames} text-(--surface-font-color) rounded-md surface`}
        >
            {children}
        </Component>
    );
}