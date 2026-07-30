export async function getBase64EncodedData(url: string): Promise<string> {
    const response = await fetch(url);
    const bytes = await response.bytes();

    let binary = "";
    const chunkSize = 0x8000;

    for (let i = 0; i < bytes.length; i += chunkSize) {
        binary += String.fromCharCode(...bytes.subarray(i, i + chunkSize));
    }

    return btoa(binary);
}

