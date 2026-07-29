"use client";

export async function cropImage(src: string, x: number, y: number, width: number, height: number) {

    const canvas = document.createElement("canvas");
    let image: HTMLImageElement;

    try {
        image = await loadImage(src);
    } catch (e) {
        return undefined;
    }

    canvas.width = width;
    canvas.height = height;

    const context = canvas.getContext("2d")!;
    context.drawImage(image, x, y, width, height, 0, 0, width, height);

    return new Promise<string | undefined>((resolve, _) => {
        canvas.toBlob((blob => {
            if (blob == null)
                return resolve(undefined);
            const url = URL.createObjectURL(blob);
            resolve(url);
        }));
    });
}

function loadImage(src: string): Promise<HTMLImageElement> {
    return new Promise((resolve, reject) => {
        const image = new Image();
        image.onload = (e) => {
            resolve(image);
        };
        image.onerror = (e) => {
            reject();
        }
        image.src = src;
    })
}