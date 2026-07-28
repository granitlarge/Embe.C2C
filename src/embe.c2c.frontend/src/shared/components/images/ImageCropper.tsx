"use client";

import { useEffect, useRef, useState } from "react";
import Button from "../buttons/Button";

type Point = {
    x: number,
    y: number
}
type Viewport = {
    x: number,
    y: number,
    width: number,
    height: number
}

type MyPointerEvent = {
    pointerId: number;
    point: Point
}

export type ImageCropperProps = {
    src: string;
    aspect: number;
    onCrop?: (crop: { x: number, y: number, width: number, height: number }) => void;
    onCancel?: () => void;
}
export default function ImageCropper({ onCrop, onCancel, src, aspect }: ImageCropperProps) {

    const scrollSpeedPixels = 100;
    const pointersHistoryRef = useRef(new Map<number, MyPointerEvent>());
    const pointersRef = useRef(new Map<number, MyPointerEvent>());

    const containerRef = useRef<HTMLDivElement>(null);
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const cropperRef = useRef<HTMLDivElement>(null);

    const grabbingContainerRef = useRef(false);
    const pinchingContainerRef = useRef(false);
    const grabbingCropperRef = useRef(false);

    const [image, setImage] = useState<HTMLImageElement | null>(null);
    const [imageWidth, setImageWidth] = useState(1);
    const [imageHeight, setImageHeight] = useState(1);

    const [containerWidth, setContainerWidth] = useState(1);
    const containerHeight = containerWidth * imageHeight / imageWidth;

    const [cropperWidth, setCropperWidth] = useState(1);
    const [cropperHeight, setCropperHeight] = useState(1);

    const [viewport, setViewport] = useState<Viewport | null>(null);

    const cropperOffsetRef = useRef({ x: 0, y: 0 });
    const [cropperX, setCropperX] = useState(0);
    const [cropperY, setCropperY] = useState(0);

    useEffect(() => {

        const onWheel = (e: WheelEvent) => {
            e.preventDefault();
            e.stopPropagation();
            onScroll(e.clientX, e.clientY, e.deltaY < 0 ? "in" : "out");
        }

        const onCropperPointerDownCallback = (e: PointerEvent) => {
            e.preventDefault();
            if (!(Math.abs(cropperWidth - containerWidth) < 10 && Math.abs(cropperHeight - containerHeight) < 10)) {
                e.stopPropagation();
            }
            onCropperPointerDown(e.clientX, e.clientY);
        }

        const onContainerPointerDownCallback = (e: PointerEvent) => {
            e.preventDefault();
            e.stopPropagation();
            pointersRef.current.set(e.pointerId, { pointerId: e.pointerId, point: { x: e.clientX, y: e.clientY } });
            if (pointersRef.current.size === 2) {
                pinchingContainerRef.current = true;
            } else {
                grabbingContainerRef.current = true;
                pinchingContainerRef.current = false;
            }
        }

        const onContainerPointerMoveCallback = (e: PointerEvent) => {
            e.preventDefault();
            e.stopPropagation();
            pointersRef.current.set(e.pointerId, { pointerId: e.pointerId, point: { x: e.clientX, y: e.clientY } });
            if (pinchingContainerRef.current === true) {
                onPinch(e);
            } else if (grabbingContainerRef.current === true || grabbingCropperRef.current === true) {
                onMove(e);
            }
        }

        cropperRef.current?.addEventListener("pointerdown", onCropperPointerDownCallback, { passive: false });
        cropperRef.current?.addEventListener("wheel", onWheel, { passive: false });

        canvasRef.current?.addEventListener("wheel", onWheel, { passive: false });

        containerRef.current?.addEventListener("pointerdown", onContainerPointerDownCallback, { passive: false });
        containerRef.current?.addEventListener("pointermove", onContainerPointerMoveCallback, { passive: false });

        window.addEventListener("pointerup", onWindowPointerUp);

        return () => {

            cropperRef.current?.removeEventListener("pointerdown", onCropperPointerDownCallback);
            cropperRef.current?.removeEventListener("wheel", onWheel);

            canvasRef.current?.removeEventListener("wheel", onWheel);

            containerRef.current?.removeEventListener("pointerdown", onContainerPointerDownCallback);
            containerRef.current?.removeEventListener("pointermove", onContainerPointerMoveCallback);

            window.removeEventListener("pointerup", onWindowPointerUp);

        }

    }, [
        onScroll,
        onCropperPointerDown,
        onPinch,
        onMove
    ])

    useEffect(() => {

        const image = new Image();
        image.onload = () => {
            setImageWidth(image.width);
            setImageHeight(image.height);
            setImage(image);
            setViewport({ x: 0, y: 0, width: image.width, height: image.height });
        }
        image.src = src;

    }, [src]);

    useEffect(() => {

        if (!containerRef.current)
            return;

        setContainerWidth(containerRef.current.clientWidth);

        const containerResizeObserver = new ResizeObserver(() => {
            if (!containerRef.current)
                return;
            setContainerWidth(containerRef.current.clientWidth);
        })

        containerResizeObserver.observe(containerRef.current);
        return () => {
            containerResizeObserver.disconnect();
        }

    }, [])

    useEffect(() => {

        if (!canvasRef.current)
            return;

        const context = canvasRef.current.getContext("2d");
        if (!context)
            return;
        if (!image)
            return;
        if (imageWidth === 1)
            return;
        if (!viewport)
            return;

        context.clearRect(0, 0, containerWidth, containerHeight);
        context.drawImage(image, viewport.x, viewport.y, viewport.width, viewport.height, 0, 0, containerWidth, containerHeight);

    }, [image, containerWidth, viewport, imageWidth]);

    useEffect(() => {

        if (imageWidth === 1 || imageHeight === 1)
            return;

        const imageContainerRatio = imageWidth / containerWidth;
        let cropperWidth = aspect;
        let cropperHeight = 1;
        if (imageWidth < imageHeight) {
            cropperWidth = imageWidth / imageContainerRatio;
            cropperHeight = imageWidth / imageContainerRatio;
        } else {
            cropperWidth = imageHeight / imageContainerRatio;
            cropperHeight = imageHeight / imageContainerRatio;
        }

        cropperWidth = Math.max(Math.min(cropperWidth, containerWidth), aspect > 1 ? aspect : 1);
        cropperHeight = Math.max(Math.min(cropperHeight, containerHeight), aspect > 1 ? 1 : 1 / aspect);

        setCropperWidth(cropperWidth);
        setCropperHeight(cropperHeight);

    }, [aspect, imageWidth, imageHeight, containerWidth])

    function onScroll(windowX: number, windowY: number, direction: "in" | "out" | "none") {
        if (!viewport)
            return;

        const widthScrollSpeed = scrollSpeedPixels;
        const heightScrollSpeed = widthScrollSpeed / (imageWidth / imageHeight);
        let newViewportWidth;
        let newViewportHeight;
        if (direction === "in") {
            newViewportWidth = Math.max(viewport.width - widthScrollSpeed, imageWidth / imageHeight);
            newViewportHeight = Math.max(viewport.height - heightScrollSpeed, 1);
        } else if (direction === "out") {
            newViewportWidth = Math.min(viewport.width + widthScrollSpeed, imageWidth);
            newViewportHeight = Math.min(viewport.height + heightScrollSpeed, imageHeight);
        } else {
            newViewportWidth = viewport.width;
            newViewportHeight = viewport.height;
        }

        if (!containerRef.current)
            return;

        const centerX = containerRef.current.getBoundingClientRect().left + containerRef.current.getBoundingClientRect().width / 2;
        const centerY = containerRef.current.getBoundingClientRect().top + containerRef.current.getBoundingClientRect().height / 2;

        const dx = (centerX - windowX) * newViewportWidth / viewport.width;
        const dy = (centerY - windowY) * newViewportHeight / viewport.height;

        centerViewport(windowX + dx, windowY + dy, newViewportWidth, newViewportHeight);
    }

    function onMove(e: PointerEvent) {

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
                pointersHistoryRef.current.set(e.pointerId, { pointerId: e.pointerId, point: { x: e.clientX, y: e.clientY } });
                return;
            }
            if (!viewport)
                return;

            const history = pointersHistoryRef.current.get(e.pointerId)!;
            const current = e;

            const dx = current.clientX - history.point.x;
            const dy = current.clientY - history.point.y;

            // We need to find the new center.
            const cx = containerRef.current.getBoundingClientRect().left + containerRef.current.getBoundingClientRect().width / 2;
            const cy = containerRef.current.getBoundingClientRect().top + containerRef.current.getBoundingClientRect().height / 2;

            const newCenterX = cx - dx;
            const newCenterY = cy - dy;

            centerViewport(newCenterX, newCenterY, viewport.width, viewport.height);
            pointersHistoryRef.current.set(e.pointerId, { pointerId: e.pointerId, point: { x: e.clientX, y: e.clientY } });
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

    function onCropperPointerDown(windowX: number, windowY: number) {

        if (!cropperRef.current)
            return;

        const cropperBoundingClientRect = cropperRef.current.getBoundingClientRect();
        const isWithinCropper = cropperBoundingClientRect.x <= windowX && cropperBoundingClientRect.x + cropperBoundingClientRect.width >= windowX &&
            cropperBoundingClientRect.y <= windowY && cropperBoundingClientRect.y + cropperBoundingClientRect.height >= windowY;

        if (isWithinCropper) {
            const offsetX = windowX - cropperBoundingClientRect.x;
            const offsetY = windowY - cropperBoundingClientRect.y;
            cropperOffsetRef.current = { x: offsetX, y: offsetY };
        } else {
            cropperOffsetRef.current = { x: 0, y: 0 };
        }

        grabbingCropperRef.current = true;

    }

    function onWindowPointerUp(e: PointerEvent) {

        if (pointersRef.current.size !== 2)
            pinchingContainerRef.current = false;

        grabbingCropperRef.current = false;
        grabbingContainerRef.current = false;

        cropperOffsetRef.current = { x: 0, y: 0 };

        pointersRef.current.delete(e.pointerId);
        pointersHistoryRef.current.delete(e.pointerId);


    }

    function distance(p1: { x: number, y: number }, p2: { x: number, y: number }) {
        const distance = Math.sqrt(Math.pow(p1.x - p2.x, 2) + Math.pow(p1.y - p2.y, 2));
        return distance;
    }

    function onPinch(e: PointerEvent) {

        if (!pinchingContainerRef.current) {
            return;
        }

        if (pointersRef.current.size !== 2)
            return;

        if (!containerRef.current)
            return;

        const t1 = Array.from(pointersRef.current.values())[0];
        const t2 = Array.from(pointersRef.current.values())[1];

        const h1 = pointersHistoryRef.current.get(t1.pointerId);
        const h2 = pointersHistoryRef.current.get(t2.pointerId);

        if (!h1 || !h2) {
            if (!pointersHistoryRef.current.get(e.pointerId)) {
                pointersHistoryRef.current.set(e.pointerId, { pointerId: e.pointerId, point: { x: e.clientX, y: e.clientY } });
            }
            return;
        }

        const currentDistance = distance({ x: t1.point.x, y: t1.point.y }, { x: t2.point.x, y: t2.point.y });
        const historyDistance = distance({ x: h1.point.x, y: h1.point.y }, { x: h2.point.x, y: h2.point.y });

        const direction = currentDistance < historyDistance ? "out" : "in";

        if (!containerRef.current)
            return;

        const centerX = containerRef.current.getBoundingClientRect().left + containerRef.current.getBoundingClientRect().width / 2;
        const centerY = containerRef.current.getBoundingClientRect().top + containerRef.current.getBoundingClientRect().height / 2;

        onScroll(centerX, centerY, direction);
        pointersHistoryRef.current.set(e.pointerId, { pointerId: e.pointerId, point: { x: e.clientX, y: e.clientY } });
    }

    function centerViewport(windowX: number, windowY: number, width: number, height: number) {

        setViewport(viewport => {

            if (!containerRef.current) {
                return viewport;
            }

            if (!viewport)
                return viewport;

            const containerX = windowX - containerRef.current.getBoundingClientRect().left;
            const containerY = windowY - containerRef.current.getBoundingClientRect().top;

            const viewportX = containerX * viewport.width / containerWidth;
            const viewportY = containerY * viewport.width / containerWidth;

            const imageX = viewport.x + viewportX;
            const imageY = viewport.y + viewportY;

            const x = imageX - width / 2;
            const y = imageY - height / 2;

            const safeX = Math.max(Math.min(x, imageWidth - width), 0);
            const safeY = Math.max(Math.min(y, imageHeight - height), 0);

            return {
                x: safeX,
                y: safeY,
                width: width,
                height: height
            }

        });

    }

    function onSave() {

        const viewportToContainerRatio = viewport!.width / containerWidth;

        const offsetX = (viewport!.x) + cropperX * viewportToContainerRatio
        const offsetY = (viewport!.y) + cropperY * viewportToContainerRatio;

        onCrop?.({ x: offsetX, y: offsetY, width: cropperWidth * viewportToContainerRatio, height: cropperHeight * viewportToContainerRatio});

    }

    return (
        <div className="w-full touch-none flex flex-col gap-1">
            <div
                ref={containerRef}
                className="relative w-full"
            >
                <canvas
                    ref={canvasRef}
                    className="w-full"
                    style={{ height: containerHeight, width: containerWidth }}
                    width={containerWidth}
                    height={containerHeight}
                >

                </canvas>
                {
                    <div
                        ref={cropperRef}
                        style={{ width: cropperWidth ?? 0, height: cropperHeight ?? 0, top: cropperY, left: cropperX }}
                        className="absolute border border-solid border-gray-300 border-5 rounded-lg flex gap-0 flex-wrap justify-start"
                    >
                    </div>
                }
            </div>
            <div className="flex gap-1">
                <Button intent="save" onClick={onSave}>crop</Button>
            </div>
        </div>
    )
}