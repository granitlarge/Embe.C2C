"use client";

import Button from "@/src/shared/components/buttons/Button";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import Link from "next/link";
import { useState } from "react";
import * as z from "zod";
import { useRouter } from "next/navigation";
import { SignIn } from "../actions/sign-in/actions";
import { SignInError } from "../actions/sign-in/types";
import Surface from "@/src/shared/components/surfaces/Surface";

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

            const error = await SignIn(userName!, password!);
            if (error !== undefined) {
                switch (error) {
                    case SignInError.InvalidCredentials:
                        setError("invalid credentials");
                        break;
                    default:
                        setError("an unknown error occurred");
                }
            } else {
                router.replace("/protected/swipe");
            }

        }

    }

    function clearErrors() {
        setUsernameError(undefined);
        setPasswordError(undefined);
        setError(undefined);
    }

    const passwordLabel = <span>password<Link href="/public/forgot-password" title="Forgot Password?">?</Link></span>;
    return (
        <Surface className="form flex flex-col gap-4 p-1 w-[600px] max-w-full">
            <TextInput label="email" type="email" placeholder="name@example.com" value={userName} valid={usernameError === undefined} errorMessage={usernameError} onChange={(un: string) => {
                setUsername(un);
                clearErrors();
            }} />
            <TextInput label={passwordLabel} type="password" placeholder="***********" value={password} valid={passwordError === undefined} errorMessage={passwordError} onChange={(pw: string) => {
                setPassword(pw);
                clearErrors();
            }} />
            {error && <span className="error-message">{error}</span>}
            <Button className="w-full" onClick={login}>login</Button>
        </Surface>
    )

}