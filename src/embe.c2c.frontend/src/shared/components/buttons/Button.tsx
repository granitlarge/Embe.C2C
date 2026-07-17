"use client";

import { useState } from "react";

export type ButtonIntent = "save" | "destructive" | "cancel" | "preview" | "navigate" | "create" | "default" | "none";
export type ButtonProps = Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, "onClick"> & {
    onClick?: () => (void | Promise<void>);
    intent?: ButtonIntent;
}

export default function Button({ onClick, intent = "default", ...props }: ButtonProps) {

    const [loading, setLoading] = useState(false);
    const classNames = [
        props.className,
        "button",
        intent === "save" ? "button-save" :
            intent === "destructive" ? "button-destructive" :
                intent === "cancel" ? "button-cancel" :
                    intent === "preview" ? "button-preview" :
                        intent === "default" ? "button-default" :
                            intent === "navigate" ? "button-navigate" :
                                intent === "create" ? "button-create" : ""
    ].filter(Boolean).join(" ");

    return (
        <button {...props} className={`${classNames} active:scale-95`} onClick={async () => {
            if (loading) {
                return;
            }
            setLoading(true);
            const result = onClick?.();
            if (result instanceof Promise) {
                try {
                    await result;
                    setLoading(false);
                } catch (e) {
                    setLoading(false);
                    throw e;
                }
            } else {
                setLoading(false);
            }
        }}>
            {loading ? "loading..." : props.children}
        </button>
    );

}