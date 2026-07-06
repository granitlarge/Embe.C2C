"use client";

import { useState } from "react";

export type ButtonVariant = "primary" | "secondary";
export type ButtonProps = Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, "onClick"> & {
    onClick?: () => (void | Promise<void>);
    variant?: ButtonVariant;
}

export default function Button({ onClick, variant = "primary", ...props }: ButtonProps) {

    const [loading, setLoading] = useState(false);
    const classNames = [
        props.className,
        variant === "primary" ? "button-primary" :
            variant === "secondary" ? "button-secondary" : ""
    ].filter(Boolean).join(" ");

    return (
        <button {...props} className={classNames} onClick={async () => {
            setLoading(true);
            const result = onClick?.();
            if (result instanceof Promise) {
                await result;
            }
            setLoading(false);
        }}>
            {loading ? "loading..." : props.children}
        </button>
    );
}