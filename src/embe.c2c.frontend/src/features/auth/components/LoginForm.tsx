"use client";

import Button from "@/src/components/buttons/Button";
import TextInput from "@/src/components/inputs/text-input/TextInput";
import Link from "next/link";
import { useState } from "react";
import * as z from "zod";
import { Login } from "../apis/Login";
import { useRouter } from "next/navigation";
import { LoginError } from "../types/login-error";

export default function LoginForm() {

    const router = useRouter();
    const validationScheme = z.object({
        userName: z.email({ message: "please enter a valid email address" }),
        password: z.string({ message: "please enter a password" })
    })

    const [userName, setUsername] = useState<string | undefined>(undefined);
    const [password, setPassword] = useState<string | undefined>(undefined);

    const [usernameError, setUsernameError] = useState<string | undefined>(undefined);
    const [passwordError, setPasswordError] = useState<string | undefined>(undefined);
    const [error, setError] = useState<string | undefined>(undefined);

    async function login() {

        const validationResult = validationScheme.safeParse({ userName, password });
        if (!validationResult.success) {
            const properties = z.treeifyError(validationResult.error).properties;
            setUsernameError(properties?.userName?.errors?.[0]);
            setPasswordError(properties?.password?.errors?.[0]);
            return;
        } else {
            clearErrors();
            const response = await Login(userName!, password!);
            if (response.success) {
                router.replace("/home");
                return;
            } else {
                const errorMessage = response.error === LoginError.InvalidCredentials ? "the username or password is incorrect" : "an unknown error occurred";
                setError(errorMessage);
            }
        }

    }

    function clearErrors() {
        setUsernameError(undefined);
        setPasswordError(undefined);
        setError(undefined);
    }

    const passwordLabel = <span>password<Link href="/forgot-password" title="Forgot Password?">?</Link></span>;
    return (
        <div className="form flex flex-col gap-4 p-8 w-[600px] max-w-full">
            <TextInput label="email" type="email" placeholder="name@example.com" value={userName} valid={usernameError === undefined} errorMessage={usernameError} onChange={(un: string) => {
                setUsername(un);
                clearErrors();
            }} />
            <TextInput label={passwordLabel} type="password" placeholder="***********" value={password} valid={passwordError === undefined} errorMessage={passwordError} onChange={(pw: string) => {
                setPassword(pw);
                clearErrors();
            }} />
            {error && <span className="error-message">{error}</span>}
            <Button className="max-w-xs" onClick={login} disabled={usernameError !== undefined || passwordError !== undefined || error !== undefined}>login</Button>
        </div>
    )

}