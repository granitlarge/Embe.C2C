"use client";

import { kMaxLength } from "buffer";
import { useCallback, useEffect, useRef, useState } from "react";

type Point = {
    x: number,
    y: number
}
export type ImageCropperProps = {
    src: string;
    width: number;
    height: number;
}
export default function ImageCropper({ src, width, height }: ImageCropperProps) {

    const scrollSpeed = .05;
    const pointersHistoryRef = useRef(new Map());
    const pointersRef = useRef(new Map());

    const containerRef = useRef<HTMLDivElement>(null);
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const cropperRef = useRef<HTMLDivElement>(null);
    const grabbingCropperRef = useRef<boolean>(false);
    const grabbingContainerRef = useRef<boolean>(false);
    const cropperOffsetRef = useRef<Point>({ x: 0, y: 0 });
    const pinchingContainerRef = useRef(false);

    const [image, setImage] = useState<HTMLImageElement | null>(null);
    const [cropperWidth, setCropperWidth] = useState(0);
    const [cropperHeight, setCropperHeight] = useState(0);
    const [imageWidth, setImageWidth] = useState(0);
    const [imageHeight, setImageHeight] = useState(0);
    const [containerWidth, setContainerWidth] = useState(0);
    const containerHeight = imageWidth !== 0 ? containerWidth * imageHeight / imageWidth : 0;

    const [cropperX, setCropperX] = useState(0);
    const [cropperY, setCropperY] = useState(0);

    const [viewport, setViewport] = useState({ x: 0, y: 0, width: imageWidth, height: imageHeight })

    useEffect(() => {

        window.addEventListener("pointerup", onWindowPointerUp);

        return () => {
            window.removeEventListener("pointerup", onWindowPointerUp);
        }

    }, [])

    useEffect(() => {

        if (!containerRef.current)
            return;

        setContainerWidth(containerRef.current.clientWidth);

        const containerResizeObserver = new ResizeObserver(() => {
            if (!containerRef.current)
                return;
            setContainerWidth(containerRef.current.clientWidth);
        })

        setContainerWidth(containerRef.current.clientWidth);
        containerResizeObserver.observe(containerRef.current);
        () => {
            containerResizeObserver.disconnect();
        }

    }, [])

    useEffect(() => {

        const image = new Image();
        image.onload = () => {
            setImage(image);
            setImageWidth(image.width);
            setImageHeight(image.height);
            setViewport({ x: 0, y: 0, width: image.width, height: image.height })
        }
        image.src = src;

    }, [src, setImage, setImageWidth, setImageHeight, setViewport]);

    useEffect(() => {

        if (!canvasRef.current)
            return;

        const context = canvasRef.current.getContext("2d");
        if (!context)
            return;
        if (!image)
            return;
        if (imageWidth === 0)
            return;

        context.clearRect(0, 0, containerWidth, containerHeight);
        context.drawImage(image, viewport.x, viewport.y, viewport.width, viewport.height, 0, 0, containerWidth, containerHeight);

    }, [image, containerWidth, viewport]);

    useEffect(() => {

        if (imageWidth === 0 || imageHeight === 0)
            return;

        const cropperWidthToHeightRatio = width / height;
        let cropperWidth;
        let cropperHeight;

        if (containerWidth < containerHeight) {
            cropperWidth = containerWidth;
            cropperHeight = cropperWidth * 1 / cropperWidthToHeightRatio;
        } else {
            cropperHeight = containerHeight;
            cropperWidth = cropperHeight * cropperWidthToHeightRatio;
        }
        setCropperWidth(cropperWidth);
        setCropperHeight(cropperHeight);
        console.log("Setting Cropper Dimensions: ", cropperWidth, cropperHeight);

    }, [width, height, imageWidth, imageHeight, containerWidth])

    function onWheel(clientX: number, clientY: number, deltaY: number, deltaX: number, deltaZ: number) {

        const direction = deltaY < 0 ? "in" : deltaY > 0 ? "out" : "none";
        let newWidth;
        let newHeight;
        if (direction === "in") {
            newWidth = Math.max(viewport.width * (1 - scrollSpeed), imageWidth / imageHeight);
            newHeight = Math.max(viewport.height * (1 - scrollSpeed), 1);
        } else if (direction === "out") {
            newWidth = Math.min(viewport.width * (1 + scrollSpeed), imageWidth);
            newHeight = Math.min(viewport.height * (1 + scrollSpeed), imageHeight);
        } else {
            newWidth = width;
            newHeight = height;
        }

        if (!containerRef.current)
            return;

        const centerX = containerRef.current.getBoundingClientRect().left + containerRef.current.getBoundingClientRect().width / 2;
        const centerY = containerRef.current.getBoundingClientRect().top + containerRef.current.getBoundingClientRect().height / 2;

        const dx = (centerX - clientX) * newWidth / viewport.width;
        const dy = (centerY - clientY) * newHeight / viewport.height;

        updateViewport(clientX + dx, clientY + dy, newWidth, newHeight);
    }

    function onMove(e: React.PointerEvent) {

        updatePosition();
        updateCropperPosition();

        function updatePosition() {

            if (!containerRef.current)
                return;
            if (grabbingContainerRef.current !== true)
                return;
            if (pointersRef.current.size !== 1)
                return;
            if (!pointersHistoryRef.current.has(e.pointerId)) {
                pointersHistoryRef.current.set(e.pointerId, e);
                return;
            }

            const history = pointersHistoryRef.current.get(e.pointerId) as React.PointerEvent;
            const current = e;

            const dx = current.clientX - history.clientX;
            const dy = current.clientY - history.clientY;

            // We need to find the new center.
            const cx = containerRef.current.getBoundingClientRect().left + containerRef.current.getBoundingClientRect().width / 2;
            const cy = containerRef.current.getBoundingClientRect().top + containerRef.current.getBoundingClientRect().height / 2;

            updateViewport(cx - dx, cy - dy, viewport.width, viewport.height);
            pointersHistoryRef.current.set(e.pointerId, e);
        }

        function updateCropperPosition() {
            if (grabbingCropperRef.current !== true)
                return;
            if (!containerRef.current)
                return;

            const containerBoundingClientRect = containerRef.current.getBoundingClientRect();

            const newCropperX = Math.max(Math.min(e.clientX - containerBoundingClientRect.left - (cropperOffsetRef.current.x), containerWidth - cropperWidth), 0);
            const newCropperY = Math.max(Math.min(e.clientY - containerBoundingClientRect.top - (cropperOffsetRef.current.y), containerHeight - cropperHeight), 0);

            setCropperX(newCropperX);
            setCropperY(newCropperY);
        }

    }

    function updateViewport(clientX: number, clientY: number, newWidth: number, newHeight: number) {

        if (!containerRef.current)
            return;

        const offsetX = clientX - containerRef.current.getBoundingClientRect().left;
        const offsetY = clientY - containerRef.current.getBoundingClientRect().top;

        const relativeX = offsetX / containerWidth;
        const relativeY = offsetY / containerHeight;

        const sourceImageX = viewport.x + viewport.width * relativeX;
        const sourceImageY = viewport.y + viewport.height * relativeY;

        const destinationImageX = newWidth / 2;
        const destinationImageY = newHeight / 2;

        const sourceOffsetX = Math.max(Math.min(sourceImageX - destinationImageX, imageWidth - newWidth), 0);
        const sourceOffsetY = Math.max(Math.min(sourceImageY - destinationImageY, imageHeight - newHeight), 0);

        setViewport({ x: sourceOffsetX, y: sourceOffsetY, width: newWidth, height: newHeight });

    }

    function onCropperPointerDown(clientX: number, clientY: number) {

        if (!cropperRef.current)
            return;

        const cropperBoundingClientRect = cropperRef.current.getBoundingClientRect();
        const isWithinCropper = cropperBoundingClientRect.x <= clientX && cropperBoundingClientRect.x + cropperBoundingClientRect.width >= clientX &&
            cropperBoundingClientRect.y <= clientY && cropperBoundingClientRect.y + cropperBoundingClientRect.height >= clientY;

        if (isWithinCropper) {
            const offsetX = clientX - cropperBoundingClientRect.x;
            const offsetY = clientY - cropperBoundingClientRect.y;
            cropperOffsetRef.current = { x: offsetX, y: offsetY };
        } else {
            cropperOffsetRef.current = { x: 0, y: 0 };
        }

        grabbingCropperRef.current = true;

    }

    function onWindowPointerUp(e: PointerEvent) {

        grabbingCropperRef.current = false;
        grabbingContainerRef.current = false;

        cropperOffsetRef.current = { x: 0, y: 0 };

        pointersRef.current.delete(e.pointerId);
        pointersHistoryRef.current.delete(e.pointerId);

    }

    function distance(p1: {x:number, y:number}, p2: {x:number,y:number}) {
        const distance = Math.sqrt(Math.pow(p1.x - p2.x, 2) + Math.pow(p1.y - p2.y, 2));
        return distance;
    }

    function onPinch(e: React.PointerEvent) {

        if (!pinchingContainerRef.current) {
            return;
        }

        if (pointersRef.current.size !== 2)
            return;

        if (!containerRef.current)
            return;

        const t1 = Array.from(pointersRef.current.values())[0] as React.PointerEvent;
        const t2 = Array.from(pointersRef.current.values())[1] as React.PointerEvent;

        const h1 = pointersHistoryRef.current.get(t1.pointerId) as React.PointerEvent | null | undefined;
        const h2 = pointersHistoryRef.current.get(t2.pointerId) as React.PointerEvent | null | undefined;

        if (!h1 || !h2) {
            if (!pointersHistoryRef.current.get(e.pointerId)) {
                pointersHistoryRef.current.set(e.pointerId, e);
            }
            return;
        }

        const currentDistance = distance({ x: t1.clientX, y: t1.clientY }, { x: t2.clientX, y: t2.clientY });
        const historyDistance = distance({ x: h1.clientX, y: h1.clientY }, { x: h2.clientX, y: h2.clientY });

        const direction = currentDistance > historyDistance ? "out" : "in";
        const scrollAmount = distance({ x: h1.clientX, y: h1.clientY }, { x: t1.clientX, y: t1.clientY });

        if (!containerRef.current)
            return;

        const centerX = containerRef.current.getBoundingClientRect().left + containerRef.current.getBoundingClientRect().width / 2;
        const centerY = containerRef.current.getBoundingClientRect().top + containerRef.current.getBoundingClientRect().height / 2;

        onWheel
            (
                centerX,
                centerY,
                direction === "in" ? -1 : 1,
                0,
                0
            );
        pointersHistoryRef.current.set(e.pointerId, e);

    }

    return (
        <div className="w-full touch-none">
            <div
                ref={containerRef}
                className="relative w-full"
                onPointerMove={(e) => {
                    if (pinchingContainerRef.current === true) {
                        onPinch(e);
                    }
                    onMove(e);
                }}
                onPointerDown={(e) => {
                    pointersRef.current.set(e.pointerId, e)
                    grabbingContainerRef.current = true;
                    if (pointersRef.current.size === 2) {
                        pinchingContainerRef.current = true;
                    } else {
                        pinchingContainerRef.current = false;
                    }
                }}
            >
                <canvas
                    ref={canvasRef}
                    className="w-full"
                    style={{ height: containerHeight }}
                    width={containerWidth}
                    height={containerHeight}
                    onWheel={(e) => {
                        onWheel(e.clientX, e.clientY, e.deltaY, e.deltaX, e.deltaZ);
                    }}
                >

                </canvas>
                {
                    <div
                        ref={cropperRef}
                        style={{ width: cropperWidth ?? 0, height: cropperHeight ?? 0, top: cropperY, left: cropperX }}
                        className="absolute bg-black opacity-50"
                        onWheel={(e) => {
                            onWheel(e.clientX, e.clientY, e.deltaY, e.deltaX, e.deltaZ);
                        }}
                        onPointerDown={(e) => { console.log("cropper pointer down"); e.stopPropagation(); onCropperPointerDown(e.clientX, e.clientY); }}
                        onMouseMove={(e) => { e.stopPropagation(); }}
                    >

                    </div>
                }
            </div>
        </div>
    )
}