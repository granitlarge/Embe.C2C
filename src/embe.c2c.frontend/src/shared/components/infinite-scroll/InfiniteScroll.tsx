"use client";

import { useEffect, useLayoutEffect, useRef, useState } from "react";
import Surface from "../surfaces/Surface";
import { Loader } from "@deemlol/next-icons";

export type Position = "start" | "end" | "center";
export type InfiniteScrollDirection = "up" | "down" | "left" | "right";
export type InfiniteScrollProps = {
    className?: string;
    children: React.ReactElement<React.LiHTMLAttributes<HTMLLIElement>>[];
    callback: () => Promise<boolean>;
    direction?: InfiniteScrollDirection;
};
export function InfiniteScroll({ className, children, callback, direction = "down" }: InfiniteScrollProps) {

    const classNames = [className].filter(Boolean).join(" ");

    const [hasMore, setHasMore] = useState(true);
    const [position, setPosition] = useState<Position>("start");
    const sentinel = useRef<HTMLLIElement>(null);
    const surface = useRef<HTMLDivElement>(null);

    useLayoutEffect(() => {

        if (!surface.current) {
            return;
        }

        if (position === "start") {
            surface.current.scrollTo({
                top: direction === "up" ? surface.current.scrollHeight : 0,
                left: direction === "left" ? surface.current.scrollWidth : 0,
            })
        } else if (position === "end") {
            surface.current.scrollTo({
                top: direction === "down" ? surface.current.scrollHeight : 0,
                left: direction === "right" ? surface.current.scrollWidth : 0,
            })
        }

    }, [children.length, direction]);

    useEffect(() => {

        const cleanupIntersactionObserver = setupIntersectionObserver();
        const cleanupScrollPositionListener = setupScrollPositionListener();

        return () => {
            cleanupIntersactionObserver();
            cleanupScrollPositionListener();
        };

        function setupScrollPositionListener(): () => void {

            if (!surface.current) {
                return () => { };
            }

            const listener = () => {
                const scrollHeight = surface.current?.scrollHeight;
                const scrollTop = surface.current?.scrollTop;
                const clientHeight = surface.current?.clientHeight;

                const scrollWidth = surface.current?.scrollWidth;
                const scrollLeft = surface.current?.scrollLeft;
                const clientWidth = surface.current?.clientWidth;

                const isAtTop = scrollTop === 0;
                const isAtBottom = scrollTop! + clientHeight! === scrollHeight;

                const isAtLeft = scrollLeft === 0;
                const isAtRight = scrollLeft! + clientWidth! === scrollWidth;

                if (direction === "down") {
                    if (isAtTop) {
                        setPosition("start");
                    } else if (isAtBottom) {
                        setPosition("end");
                    } else if (position !== "center") {
                        setPosition("center");
                    }
                } else if (direction === "up") {
                    if (isAtTop) {
                        setPosition("end");
                    } else if (isAtBottom) {
                        setPosition("start");
                    } else if (position !== "center") {
                        setPosition("center");
                    }
                } else if (direction === "right") {
                    if (isAtLeft) {
                        setPosition("start");
                    } else if (isAtRight) {
                        setPosition("end");
                    } else if (position !== "center") {
                        setPosition("center");
                    }
                } else if (direction === "left") {
                    if (isAtLeft) {
                        setPosition("end");
                    } else if (isAtRight) {
                        setPosition("start");
                    } else if (position !== "center") {
                        setPosition("center");
                    }
                }
            };

            surface.current.addEventListener("scroll", listener);

            return () => {
                surface.current?.removeEventListener("scroll", listener);
            }
        }

        function setupIntersectionObserver(): () => void {
            const options = {
                root: surface.current,
                rootMargin: "0px",
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
            return () => observer.disconnect();
        }

    }, [callback]);

    return (
        <div ref={surface} className="overflow-scroll scrollbar-none">
            <Surface as="ul" className={`${classNames}`} padding="none" variant="inherit">
                {hasMore && (direction === "up" || direction === "left") && (
                    <li ref={sentinel} className="flex justify-center">
                        <Loader className="animate-spin text-(length:--fs-secondary) mx-auto" />
                    </li>
                )}
                {children}
                {hasMore && (direction === "down" || direction === "right") && (
                    <li ref={sentinel} className="flex justify-center">
                        <Loader className="animate-spin text-(length:--fs-secondary) mx-auto" />
                    </li>
                )}
            </Surface>
        </div>
    )

}