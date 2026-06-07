export class ApiError extends Error {

    public readonly name = "ApiError";
    public readonly status: number;
    public readonly statusText: string;
    public readonly errorBody?: string;
    public readonly headers?: Headers;

    constructor(status: number, statusText: string, errorBody?: string, headers?: Headers) {
        super(`API Error: ${status} ${statusText}`);
        this.status = status;
        this.statusText = statusText;
        this.errorBody = errorBody;
        this.headers = headers;
    }


    public static async fromResponse(response: Response): Promise<ApiError> {
        const status = response.status;
        const statusText = response.statusText;
        const errorBody = await response.text();
        const headers = response.headers;
        return new ApiError(status, statusText, errorBody, headers);
    }

}