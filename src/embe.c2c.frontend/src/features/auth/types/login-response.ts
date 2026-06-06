import { LoginError } from "./login-error";

export type LoginResponse = {
    success: boolean;
    error?: LoginError;
}