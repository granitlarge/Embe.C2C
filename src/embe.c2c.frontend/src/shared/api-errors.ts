export class ApiError {
    public readonly response: Response;
    constructor(response: Response) {
        this.response = response;
    }
}