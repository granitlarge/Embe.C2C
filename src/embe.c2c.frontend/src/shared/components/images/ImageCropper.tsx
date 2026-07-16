"use client";

import { useEffect, useRef, useState } from "react";

export type ImageCropperProps = {
    src: string;
    width: number;
    height: number;
}
export default function ImageCropper({ src, width, height }: ImageCropperProps) {

    const containerRef = useRef<HTMLDivElement>(null);
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const cropperRef = useRef<HTMLDivElement>(null);

    const [image, setImage] = useState<HTMLImageElement | null>(null);
    const [cropperWidth, setCropperWidth] = useState(0);
    const [cropperHeight, setCropperHeight] = useState(0);
    const [imageWidth, setImageWidth] = useState(0);
    const [imageHeight, setImageHeight] = useState(0);
    const [containerWidth, setContainerWidth] = useState(0);
    const [containerHeight, setContainerHeight] = useState(0);

    const [cropperX, setCropperX] = useState(0);
    const [cropperY, setCropperY] = useState(0);

    console.log({container: {containerWidth, containerHeight}, image: {imageWidth, imageHeight}, cropper: {cropperWidth, cropperHeight}});

    useEffect(() => {

        if (!containerRef.current)
            return;

        const onResize = () => {
            setContainerHeight(containerRef.current?.clientHeight ?? 0);
            setContainerWidth(containerRef.current?.clientWidth ?? 0);
        };


        setContainerHeight(containerRef.current.clientHeight);
        setContainerWidth(containerRef.current.clientWidth);

        containerRef.current.addEventListener("resize", onResize);
        () => {
            containerRef.current?.removeEventListener("resize", onResize);
        }

    }, [containerRef.current])

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

        context.drawImage(image, 0, 0, containerWidth, containerWidth / (imageWidth / imageHeight));

    }, [image, containerWidth, imageWidth, imageHeight]);

    useEffect(() => {
        // the cropper dimensions depend on the dimensions requested by the caller, the container width, and the image dimensions.
        setCropperWidth(Math.min(containerWidth, width));
        setCropperHeight(Math.min(containerHeight, height));
    }, [width, height, imageWidth, imageHeight, containerWidth, containerHeight])

    useEffect(() => {

    }, [containerWidth, containerHeight])

    return (
        <div ref={containerRef} className="relative w-full">
            <canvas ref={canvasRef} className="w-full">

            </canvas>
            <div ref={cropperRef} style={{ width: cropperWidth ?? 0, height: cropperHeight ?? 0 }} className="absolute top-0 left-0 bg-black opacity-50">

            </div>
        </div>
    )
}