import { type Error } from "./apis/type"

export function getErrorMessage(error: Error | undefined) : string {

    if (error?.code.startsWith("auth")) {
        return getAuthErrorMessage(error);
    }

    return "an unknown error occurred";

}

function getAuthErrorMessage(error: Error): string {
    if (!error.code.startsWith("auth")) {
        throw new Error("Error isn't auth related");
    }

    switch (error.code){
        case "auth.invalid_credentials":
            return "there is no account associated with that e-mail/password combination";
        case "auth.no_user_with_supplied_email":
            return "there is no account associated with that e-mail";
        case "auth.locked_out":
            return "your account has been locked, reset your password to continue";
        default:
            return "an unknown auth-related error occurred"
    }

}