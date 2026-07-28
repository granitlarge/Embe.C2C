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
        <>
            {
                isLoading &&
                <div className={`${classNames} flex items-center justify-center`}>
                    <Loader className="animate-spin w-(--primary-fs) h-(--primary-fs)" />
                </div>
            }
            {
                <NextImage
                    className={`${classNames} transition-opacity duration-300 ${isLoading ? "opacity-0" : "opacity-100"}`}
                    onLoad={(e) => { setIsLoading(false); onLoad?.(e); }}
                    onError={(e) => { setIsLoading(false); onError?.(e); }}
                    {...props}
                />
            }
        </>
    )
}