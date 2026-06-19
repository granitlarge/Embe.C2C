"use client";

import { useEffect, useLayoutEffect, useRef, useState } from "react";
import Surface from "../surfaces/Surface";
import { Loader } from "@deemlol/next-icons";

export type Position = "start" | "end" | "center";
export type InfiniteScrollDirection = "up" | "down";
export type InfiniteScrollProps = {
    className?: string;
    children: React.ReactElement<React.LiHTMLAttributes<HTMLLIElement>>[];
    callback: () => Promise<boolean>;
    direction?: InfiniteScrollDirection;
};

export function InfiniteScroll({ className, children, callback, direction = "down" }: InfiniteScrollProps) {

    const classNames = [className].filter(Boolean).join(" ");

    const loadingMoreRef = useRef(false);
    const hasMoreRef = useRef(true);
    const [loadingMore, setLoadingMore] = useState(false);

    const positionRef = useRef<Position>("start");
    const surface = useRef<HTMLDivElement>(null);

    useLayoutEffect(() => {

        if (!surface.current) {
            return;
        }

        if (positionRef.current === "start") {
            surface.current.scrollTo({
                top: direction === "up" ? surface.current.scrollHeight : 0,
            })
        } else if (positionRef.current === "end") {
            surface.current.scrollTo({
                top: direction === "down" ? surface.current.scrollHeight : 0,
            })
        }

    }, [children.length, direction]);

    useEffect(() => {

        const cleanupScrollPositionListener = setupScrollPositionListener();

        return () => {
            cleanupScrollPositionListener();
        };

        function setupScrollPositionListener(): () => void {

            if (!surface.current) {
                return () => { };
            }

            const listener = async () => {
                async function loadMore() {

                    if (loadingMoreRef.current || !hasMoreRef.current) {
                        return false;
                    }

                    loadingMoreRef.current = true;
                    setLoadingMore(true);
                    try {

                        const hasMoreData = await callback();
                        hasMoreRef.current = hasMoreData;

                    } catch {

                    } finally {

                        loadingMoreRef.current = false;
                        setLoadingMore(false);

                    }

                }

                if (!surface.current)
                    return;

                const scrollHeight = surface.current.scrollHeight;
                const scrollTop = surface.current.scrollTop;
                const clientHeight = surface.current.clientHeight;

                const isAtTop = Math.abs(scrollTop - 0) < 4;
                const isAtBottom = Math.abs(scrollTop + clientHeight - scrollHeight) < 4;

                if (direction === "down") {
                    if (isAtTop) {
                        positionRef.current = "start";
                    } else if (isAtBottom) {
                        positionRef.current = "end";
                        await loadMore();
                    } else {
                        positionRef.current = "center";
                    }
                } else if (direction === "up") {
                    if (isAtTop) {
                        positionRef.current = "end";
                        await loadMore();
                    } else if (isAtBottom) {
                        positionRef.current = "start";
                    } else {
                        positionRef.current = "center";
                    }
                }
            };

            surface.current.addEventListener("scroll", listener);

            return () => {
                surface.current?.removeEventListener("scroll", listener);
            }
        }

    }, [callback]);

    return (
        <div ref={surface} className={`${classNames} overflow-scroll scrollbar-none surface-inherit`}>
            {loadingMore && (direction === "up") && (
                <li className="flex justify-center">
                    <Loader className="animate-spin text-(length:--fs-secondary) mx-auto" />
                </li>
            )}
            {children}
            {loadingMore && (direction === "down") && (
                <li className="flex justify-center">
                    <Loader className="animate-spin text-(length:--fs-secondary) mx-auto" />
                </li>
            )}
        </div>
    )

}