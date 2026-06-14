import React from "react";

export type Padding = "none" | "sm" | "md" | "lg";
export type Variant = "primary" | "secondary" | "tertiary" | "inherit";

export type SurfaceProps<T extends React.ElementType = "div"> = {
    className?: string;
    children: React.ReactNode;
    padding?: Padding;
    variant?: Variant;
    as?: T;
} & React.ComponentPropsWithoutRef<T>;

export default function Surface<T extends React.ElementType = "div">({
    className,
    children,
    padding = "md",
    variant = "inherit",
    as,
    style,
    ...props
}: SurfaceProps<T>) {

    const Component = as || "div";
    const classNames = [
        className,
        `surface-${variant}`,
        padding === "none" ? "" :
            padding === "sm" ? "p-2" :
                padding === "md" ? "p-4" :
                    "p-6"
    ]
        .filter(Boolean)
        .join(" ");
    return (
        <Component
            {...props}
            className={`${classNames} rounded-md border-(--border-color)`}
        >
            {children}
        </Component>
    );
}