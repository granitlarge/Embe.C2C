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
    const cropperOffsetRef = useRef<{ x: number, y: number }>({ x: 0, y: 0 });

    const [image, setImage] = useState<HTMLImageElement | null>(null);
    const [cropperWidth, setCropperWidth] = useState(0);
    const [cropperHeight, setCropperHeight] = useState(0);
    const [imageWidth, setImageWidth] = useState(0);
    const [imageHeight, setImageHeight] = useState(0);
    const [containerWidth, setContainerWidth] = useState(0);
    const [zoomPercentageUnits, setZoomPercentageUnits] = useState(0);
    const containerHeight = imageWidth !== 0 ? containerWidth * imageHeight / imageWidth : 0;

    const [cropperX, setCropperX] = useState(0);
    const [cropperY, setCropperY] = useState(0);

    useEffect(() => {

        window.addEventListener("pointerup", onCropperPointerUp);

        return () => {
            window.removeEventListener("pointerup", onCropperPointerUp);
        }

    }, [])

    const onMouseMove = useCallback((e: React.MouseEvent) => {

        if (grabbingCropperRef.current !== true)
            return;
        if (!containerRef.current)
            return;

        const containerBoundingClientRect = containerRef.current.getBoundingClientRect();

        const newCropperX = Math.max(Math.min(e.clientX - containerBoundingClientRect.left - (cropperOffsetRef.current.x), containerWidth - cropperWidth), 0);
        const newCropperY = Math.max(Math.min(e.clientY - containerBoundingClientRect.top - (cropperOffsetRef.current.y), containerHeight - cropperHeight), 0);

        setCropperX(newCropperX);
        setCropperY(newCropperY);

    }, [setCropperX, setCropperY, containerWidth, containerHeight, cropperWidth, cropperHeight]);

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
        }
        image.src = src;

    }, [src]);

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
        context.drawImage(image, 0, 0, imageWidth * ((100 - Math.min(zoomPercentageUnits, 90)) / 100), imageHeight * ((100 - Math.min(zoomPercentageUnits, 90)) / 100), 0, 0, containerWidth, containerHeight);

    }, [image, containerWidth, zoomPercentageUnits]);

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

    }, [width, height, imageWidth, imageHeight, containerWidth, zoomPercentageUnits])

    function onWheel(deltaY: number, deltaX: number, deltaZ: number) {
        const direction = deltaY < 0 ? "in" : "out";
        let newZoomPercentage = zoomPercentageUnits;
        if (direction === "out") {
            const newZoomPercentageCandidate = Math.max(0, zoomPercentageUnits - 10);
            if (!exceedsContainer(newZoomPercentageCandidate)) {
                newZoomPercentage = newZoomPercentageCandidate;
            }
        } else {
            const newZoomPercentageCandidate = zoomPercentageUnits + 10;
            if (!exceedsContainer(newZoomPercentageCandidate)) {
                newZoomPercentage = newZoomPercentageCandidate;
            }
        }

        setZoomPercentageUnits(newZoomPercentage);

        function exceedsContainer(newPercentage: number) {
            const exceedsContainerHeight = newPercentage / 100 * cropperHeight >= containerHeight;
            const exceedsContainerWidth = newPercentage / 100 * cropperWidth >= containerWidth;
            const exceedsContainer = exceedsContainerHeight || exceedsContainerWidth;
            return exceedsContainer;
        }
    }

    function onCropperPointerDown(e: React.MouseEvent<HTMLDivElement>) {

        if (!cropperRef.current)
            return;

        const cropperBoundingClientRect = cropperRef.current.getBoundingClientRect();
        const isWithinCropper = cropperBoundingClientRect.x <= e.clientX && cropperBoundingClientRect.x + cropperBoundingClientRect.width >= e.clientX &&
            cropperBoundingClientRect.y <= e.clientY && cropperBoundingClientRect.y + cropperBoundingClientRect.height >= e.clientY;

        if (isWithinCropper) {
            const offsetX = e.clientX - cropperBoundingClientRect.x;
            const offsetY = e.clientY - cropperBoundingClientRect.y;
            cropperOffsetRef.current = { x: offsetX, y: offsetY };
        } else {
            cropperOffsetRef.current = { x: 0, y: 0 };
        }

        grabbingCropperRef.current = true;

    }

    function onCropperPointerUp() {

        grabbingCropperRef.current = false;
        cropperOffsetRef.current = { x: 0, y: 0 };

    }

    return (
        <div
            ref={containerRef}
            className="relative w-full"
            onMouseMove={onMouseMove}

        >
            <canvas
                ref={canvasRef}
                className="w-full"
                style={{ height: containerHeight }}
                width={containerWidth}
                height={containerHeight}
                onWheel={(e) => {
                    onWheel(e.deltaY, e.deltaX, e.deltaZ);
                }}
            >

            </canvas>
            <div
                ref={cropperRef}
                style={{ width: cropperWidth ?? 0, height: cropperHeight ?? 0, top: cropperY, left: cropperX }}
                className="absolute bg-black opacity-50"
                onWheel={(e) => {
                    onWheel(e.deltaY, e.deltaX, e.deltaZ);
                }}
                onPointerDown={onCropperPointerDown}
            >

            </div>
        </div>
    )
}