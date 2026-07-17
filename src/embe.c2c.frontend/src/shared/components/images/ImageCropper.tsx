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
    const containerPinchRef = useRef(false);
    const containerLastPinchMove = useRef<React.TouchList | null>(null);

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

    function onWheel(clientX: number, clientY: number, deltaY: number, deltaX: number, deltaZ: number, speed: number) {

        const direction = deltaY < 0 ? "in" : deltaY > 0 ? "out" : "none";
        let newWidth;
        let newHeight;
        if (direction === "in") {
            newWidth = Math.max(viewport.width * (1 - speed), imageWidth * speed);
            newHeight = Math.max(viewport.height * (1 - speed), imageHeight * speed);
        } else if (direction === "out") {
            newWidth = Math.min(viewport.width * (1 + speed), imageWidth);
            newHeight = Math.min(viewport.height * (1.0 + speed), imageHeight);
        } else {
            newWidth = width;
            newHeight = height;
        }

        updateViewport(clientX, clientY, newWidth, newHeight);
    }

    function onMove(clientX: number, clientY: number) {

        updateZoomPosition();
        updateCropperPosition();

        function updateZoomPosition() {
            if (grabbingContainerRef.current !== true)
                return;
            updateViewport(clientX, clientY, viewport.width, viewport.height);
        }

        function updateCropperPosition() {
            if (grabbingCropperRef.current !== true)
                return;
            if (!containerRef.current)
                return;

            const containerBoundingClientRect = containerRef.current.getBoundingClientRect();

            const newCropperX = Math.max(Math.min(clientX - containerBoundingClientRect.left - (cropperOffsetRef.current.x), containerWidth - cropperWidth), 0);
            const newCropperY = Math.max(Math.min(clientY - containerBoundingClientRect.top - (cropperOffsetRef.current.y), containerHeight - cropperHeight), 0);

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

    function onWindowPointerUp() {

        grabbingCropperRef.current = false;
        cropperOffsetRef.current = { x: 0, y: 0 };
        grabbingContainerRef.current = false;

    }

    function onContainerPinchMove(e: React.TouchEvent<HTMLDivElement>) {

        if (containerPinchRef.current !== true) {
            return;
        }

        if (!containerLastPinchMove.current) {
            containerLastPinchMove.current = e.touches;
            return;
        }

        const t1 = e.touches[0];
        const t2 = e.touches[1];

        const p1 = containerLastPinchMove.current[0].identifier === t1.identifier ? containerLastPinchMove.current[0] : containerLastPinchMove.current[0];
        const p2 = containerLastPinchMove.current[0].identifier !== p1.identifier ? containerLastPinchMove.current[0] : containerLastPinchMove.current[1];

        // We need to determine whether the distance between the pinches has increased or decreased (this gives us the direction of the zoom)
        // we then determine by how much it has increased, and whether the center of the pinch has changed (which determines where we zoom)

        const prevDistance = Math.sqrt(Math.pow(p1.clientX - p2.clientX, 2) + Math.pow(p1.clientY - p2.clientY, 2));
        const distance = Math.sqrt(Math.pow(t1.clientX - t2.clientX, 2) + Math.pow(t1.clientY - t2.clientY, 2));

        const direction = distance > prevDistance ? "in" : "out";

        const newCenterX = t1.clientX + Math.abs(t1.clientX - t2.clientX) / 2;
        const newCenterY = t1.clientY + Math.abs(t1.clientY - t2.clientY) / 2;

        onWheel(newCenterX, newCenterY, direction === "in" ? -1 : 1, 0, 0, 0.01);
        containerLastPinchMove.current = e.touches;

    }

    return (
        <div className="w-full touch-none">
            <div
                ref={containerRef}
                className="relative w-full"
                onMouseMove={(e) => { onMove(e.clientX, e.clientY); }}
                onTouchMove={(e) => {
                    if (e.touches.length === 1) {

                        onMove(e.touches[0].clientX, e.touches[0].clientY);

                    } else if (e.touches.length === 2) {

                        onContainerPinchMove(e);

                    }
                }}
                onTouchStart={(e) => {
                    if (e.touches.length === 2) {
                        containerPinchRef.current = true;
                    }
                }}
                onTouchEnd={(e) => {
                    containerPinchRef.current = false;
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
                        onWheel(e.clientX, e.clientY, e.deltaY, e.deltaX, e.deltaZ, 0.05);
                    }}
                >

                </canvas>
                {
                    false &&
                    <div
                        ref={cropperRef}
                        style={{ width: cropperWidth ?? 0, height: cropperHeight ?? 0, top: cropperY, left: cropperX }}
                        className="absolute bg-black opacity-50"
                        onWheel={(e) => {
                            onWheel(e.clientX, e.clientY, e.deltaY, e.deltaX, e.deltaZ, 0.05);
                        }}
                        onPointerDown={(e) => { console.log("cropper pointer down"); e.stopPropagation(); onCropperPointerDown(e.clientX, e.clientY); }}
                        onTouchStart={(e) => e.touches.length === 1 && onCropperPointerDown(e.touches[0].clientX, e.touches[0].clientY)}
                        onMouseMove={(e) => { e.stopPropagation(); }}
                    >

                    </div>
                }
            </div>
        </div>
    )
}