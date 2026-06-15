"use client";

import { useEffect, useLayoutEffect, useRef, useState } from "react";
import Surface from "../surfaces/Surface";
import { Loader } from "@deemlol/next-icons";

export type InfiniteScrollDirection = "up/left" | "down/right";
export type InfiniteScrollProps = {
    className?: string;
    children: React.ReactElement<React.LiHTMLAttributes<HTMLLIElement>>[];
    callback: () => Promise<boolean>;
    direction?: InfiniteScrollDirection;
};
export function InfiniteScroll({ className, children, callback, direction = "down/right" }: InfiniteScrollProps) {

    const classNames = [className].filter(Boolean).join(" ");

    const [hasMore, setHasMore] = useState(true);
    const sentinel = useRef<HTMLLIElement>(null);
    const surface = useRef<HTMLDivElement>(null);

    useLayoutEffect(() => {

        if (!surface.current)
            return;
        surface.current.scrollTo({
            top: direction === "up/left" ? surface.current.scrollHeight : 0,
        })

    }, [direction]);

    useEffect(() => {

        const options = {
            root: surface.current,
            rootMargin: "100px",
            scrollMargin: "0px",
            threshold: 0
        }

        const observer = new IntersectionObserver(([entry]) => {
            if (entry.isIntersecting) {
                callback().then(hasMore => {
                    setHasMore(hasMore)
                    if (!hasMore && sentinel.current) { observer.unobserve(sentinel.current) }
                });
            }
        }, options);

        if (sentinel.current) {
            observer.observe(sentinel.current);
        }

        return () => {
            observer.disconnect();
        };

    }, [callback]);

    return (
        <div ref={surface} className="overflow-scroll scrollbar-none">
            <Surface as="ul" className={`${classNames}`} padding="none" variant="inherit">
                {hasMore && direction === "up/left" && (
                    <li ref={sentinel} className="flex justify-center">
                        <Loader className="animate-spin text-(length:--fs-secondary) mx-auto" />
                    </li>
                )}
                {children}
                {hasMore && direction === "down/right" && (
                    <li ref={sentinel} className="flex justify-center">
                        <Loader className="animate-spin text-(length:--fs-secondary) mx-auto" />
                    </li>
                )}
            </Surface>
        </div>
    )

}