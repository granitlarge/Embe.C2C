"use client";

import { useEffect, useRef, useState } from "react";

export type InfiniteScrollProps = {
    className?: string;
    children: React.ReactElement<React.LiHTMLAttributes<HTMLLIElement>>[];
    callback: () => Promise<boolean>;
};
export function InfiniteScroll({ className, children, callback }: InfiniteScrollProps) {

    const classNames = [className].filter(Boolean).join(" ");

    const [hasMore, setHasMore] = useState(true);
    const sentinel = useRef<HTMLLIElement>(null);

    useEffect(() => {

        const options = {
            root: null,
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

    }, [callback, sentinel]);

    return (
        <ul className={`${classNames}`}>
            {children}
            {hasMore && (
                <li ref={sentinel} key={children.length} className="flex justify-center">
                    <span className="text-(length:--fs-primary) mx-auto">loading...</span>
                </li>
            )}
        </ul>
    )

}