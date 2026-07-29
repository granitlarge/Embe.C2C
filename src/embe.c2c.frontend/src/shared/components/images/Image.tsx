"use client";

import { Loader } from "@deemlol/next-icons";
import { default as NextImage } from "next/image";
import type { ImageProps as NextImageProps } from "next/image";
import { useState } from "react";

type OmitProps = "className" | "onLoad" | "onError";
export type ImageProps = Omit<NextImageProps, OmitProps> & {
    className?: string;
    onLoad?: (event: React.SyntheticEvent<HTMLImageElement, Event>) => void;
    onError?: (event: React.SyntheticEvent<HTMLImageElement, Event>) => void;
}
export default function Image({ className, onLoad, onError, ...props }: ImageProps) {

    const [isLoading, setIsLoading] = useState(true);
    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <div className="relative max-w-[100%] max-h-[100%] overflow-hidden">
            {isLoading && (
                <div className={`${classNames} absolute inset-0 flex items-center justify-center`}>
                    <Loader className="animate-spin w-(--primary-fs) h-(--primary-fs)" />
                </div>
            )}

            <NextImage
                {...props}
                className={`${classNames} transition-opacity ${isLoading ? "opacity-0" : "opacity-100"}`}
                onLoad={(e) => {
                    setIsLoading(false);
                    onLoad?.(e);
                }}
                onError={(e) => {
                    setIsLoading(false);
                    onError?.(e);
                }}
            />
        </div>
    )
}