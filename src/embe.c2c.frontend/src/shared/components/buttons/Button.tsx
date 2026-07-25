"use client";

import { Loader } from "@deemlol/next-icons";
import { useEffect, useRef, useState } from "react";

export type ButtonIntent = "save" | "destructive" | "cancel" | "preview" | "navigate" | "create" | "default" | "none";
export type ButtonProps = Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, "onClick"> & {
    onClick?: () => (void | Promise<void>);
    intent?: ButtonIntent;
}

export default function Button({ onClick, intent = "none", ...props }: ButtonProps) {

    const buttonRef = useRef<HTMLButtonElement>(null);
    const buttonDimensionsRef = useRef<{
        width: number,
        height: number,
        padding: {
            top: number,
            bottom: number,
            left: number,
            right: number
        }
    }>({ width: 0, height: 0, padding: { top: 0, bottom: 0, left: 0, right: 0 } });

    const loadingRef = useRef(false);
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

    useEffect(() => {

        if (!buttonRef.current || !buttonDimensionsRef.current)
            return;

        const resizeObserver = new ResizeObserver(() => {
            if (!buttonRef.current)
                return;
            const computedStyle = getComputedStyle(buttonRef.current)
            buttonDimensionsRef.current = {
                width: parseFloat(computedStyle.width) - parseFloat(computedStyle.paddingRight) - parseFloat(computedStyle.paddingLeft) - parseFloat(computedStyle.borderLeft) - parseFloat(computedStyle.borderRight),
                height: parseFloat(computedStyle.height) - parseFloat(computedStyle.paddingTop) - parseFloat(computedStyle.paddingBottom) - parseFloat(computedStyle.borderBottom) - parseFloat(computedStyle.borderTop),
                padding: {
                    top: parseFloat(computedStyle.paddingTop),
                    bottom: parseFloat(computedStyle.paddingBottom),
                    left: parseFloat(computedStyle.paddingLeft),
                    right: parseFloat(computedStyle.paddingRight)
                }
            };
        });

        resizeObserver.observe(buttonRef.current);
        return () => {
            resizeObserver.disconnect();
        }

    }, [])

    return (
        <button ref={buttonRef} {...props} className={`${classNames} active:scale-95`} onClick={async () => {
            if (loadingRef.current) {
                return;
            }
            loadingRef.current = true;
            setLoading(true);
            const result = onClick?.();
            try {
                await result;
            } finally {
                setLoading(false);
                loadingRef.current = false;
            }
        }}>
            {
                loading ?
                    <div className="flex justify-center items-center"
                        style={{
                            width: buttonDimensionsRef.current.width,
                            height: buttonDimensionsRef.current.height,
                        }}
                    >
                        <Loader
                            className="animate-spin w-(--primary-fs) h-(--primary-fs)"
                        />
                    </div> : props.children
            }
        </button>
    );

}