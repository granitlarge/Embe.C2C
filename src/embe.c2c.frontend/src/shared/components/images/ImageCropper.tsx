"use client";

import { useCallback, useEffect, useRef, useState } from "react";

export type ImageCropperProps = {
    src: string;
    width: number;
    height: number;
}
export default function ImageCropper({ src, width, height }: ImageCropperProps) {

    const containerRef = useRef<HTMLDivElement>(null);
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const cropperRef = useRef<HTMLDivElement>(null);
    const grabbingCropperRef = useRef<boolean>(false);
    const grabbingContainerRef = useRef<boolean>(false);
    const cropperOffsetRef = useRef<{ x: number, y: number }>({ x: 0, y: 0 });

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

    const onMouseMoveUpdateCropperPosition = useCallback((clientX: number, clientY: number) => {

        if (grabbingCropperRef.current !== true)
            return;
        if (!containerRef.current)
            return;

        const containerBoundingClientRect = containerRef.current.getBoundingClientRect();

        const newCropperX = Math.max(Math.min(clientX - containerBoundingClientRect.left - (cropperOffsetRef.current.x), containerWidth - cropperWidth), 0);
        const newCropperY = Math.max(Math.min(clientY - containerBoundingClientRect.top - (cropperOffsetRef.current.y), containerHeight - cropperHeight), 0);

        setCropperX(newCropperX);
        setCropperY(newCropperY);

    }, [setCropperX, setCropperY, containerWidth, containerHeight, cropperWidth, cropperHeight]);

    const onMousemoveUpdatedZoomPosition = useCallback((clientX: number, clientY: number) => {
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

    }, [image, containerWidth, viewport.width, viewport.height]);

    useEffect(() => {

        // At this point, we have have scaled down the image to fit the full width of the parent.
        // We need to scalea the cropper as well.
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

        if (!containerRef.current)
            return;

        const direction = deltaY < 0 ? "in" : "out";
        let newWidth;
        let newHeight;
        if (direction === "in") {
            newWidth = Math.max(viewport.width * .90, imageWidth * .10);
            newHeight = Math.max(viewport.height * .90, imageHeight * 0.10);
        } else {
            newWidth = Math.min(viewport.width * 1.10, imageWidth);
            newHeight = Math.min(viewport.height * 1.10, imageHeight);
        }

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

    function onWindowPointerUp() {

        grabbingCropperRef.current = false;
        cropperOffsetRef.current = { x: 0, y: 0 };
        grabbingContainerRef.current = false;

    }

    return (
        <div
            ref={containerRef}
            className="relative w-full touch-none"
            onMouseMove={(e) => { onMouseMoveUpdateCropperPosition(e.clientX, e.clientY); onMousemoveUpdatedZoomPosition(e.clientX, e.clientY); }}
            onTouchMove={(e) => {
                if (e.touches.length === 1) {
                    onMouseMoveUpdateCropperPosition(e.touches[0].clientX, e.touches[0].clientY);
                    onMousemoveUpdatedZoomPosition(e.touches[0].clientX, e.touches[0].clientY);
                }
            }}
            onPointerDown={() => {
                grabbingContainerRef.current = true;
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
            <div
                ref={cropperRef}
                style={{ width: cropperWidth ?? 0, height: cropperHeight ?? 0, top: cropperY, left: cropperX }}
                className="absolute bg-black opacity-50"
                onWheel={(e) => {
                    onWheel(e.clientX, e.clientY,  e.deltaY, e.deltaX, e.deltaZ);
                }}
                onPointerDown={(e) => {e.stopPropagation(); onCropperPointerDown(e.clientX, e.clientY);}}
                onTouchStart={(e) => e.touches.length === 1 && onCropperPointerDown(e.touches[0].clientX, e.touches[0].clientY)}
            >

            </div>
        </div>
    )
}