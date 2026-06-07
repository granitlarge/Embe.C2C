export class ApiError extends Error {

    public readonly name = "ApiError";
    public readonly status: number;
    public readonly statusText: string;
    public readonly errorBody?: string;

    constructor(status: number, statusText: string, errorBody?: string) {
        super(`API Error: ${status} ${statusText}`);
        this.status = status;
        this.statusText = statusText;
        this.errorBody = errorBody;
    }


    public static async fromResponse(response: Response): Promise<ApiError> {
        const status = response.status;
        const statusText = response.statusText;
        const errorBody = await response.text();
        return new ApiError(status, statusText, errorBody);
    }

}