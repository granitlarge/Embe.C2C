import React from "react";

export type Padding = "none" | "sm" | "md" | "lg";

export type SurfaceProps<T extends React.ElementType = "div"> = {
    className?: string;
    children: React.ReactNode;
    padding?: Padding;
    as?: T;
    transparent?: boolean;
} & React.ComponentPropsWithoutRef<T>;

export default function Surface<T extends React.ElementType = "div">({
    className,
    children,
    padding = "md",
    as,
    transparent,
    ...props
}: SurfaceProps<T>) {
    const Component = as || "div";

    const classNames = [
        className,
        padding === "none" ? "p-0" :
            padding === "sm" ? "p-2" :
                padding === "md" ? "p-4" :
                    "p-6",
        transparent ? "bg-transparent" : "bg-(--surface)",
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