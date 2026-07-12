"use client";

import Button from "@/src/shared/components/buttons/Button";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import { useState } from "react";
import * as z from "zod";
import { signIn } from "../actions/sign-in/actions";
import { SignInError } from "../actions/sign-in/types";
import Surface from "@/src/shared/components/surfaces/Surface";
import Link from "@/src/shared/components/Links/Link";
import { useRouter } from "nextjs-toploader/app";

export type LoginFormProps = {
    className?: string;
}

export default function LoginForm({ className }: LoginFormProps) {

    const classNames = [className].filter(Boolean).join(" ");

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

            const error = await signIn(userName!, password!);
            if (error !== undefined) {

                switch (error) {
                    case SignInError.InvalidCredentials:
                        setError("invalid credentials");
                        break;
                    default:
                        setError("an unknown error occurred");
                }

            } else {

                router.push("/protected/search");

            }

        }

    }

    function clearErrors() {
        setUsernameError(undefined);
        setPasswordError(undefined);
        setError(undefined);
    }
    const passwordLabel = <>password<Link href="/public/forgot-password" title="Forgot Password?">?</Link></>;
    return (
        <Surface className={`form w-[600px] max-w-full ${classNames}`} variant="secondary">
            <TextInput
                label="email"
                type="email"
                placeholder="name@example.com"
                initialValue={userName}
                errorMessage={usernameError}
                onBlur={(un: string) => {
                    setUsername(un);
                    clearErrors();
                }} />
            <TextInput
                label={passwordLabel} type="password" placeholder="***********" initialValue={password} errorMessage={passwordError} onBlur={(pw: string) => {
                    setPassword(pw);
                    clearErrors();
                }} />
            {error && <span className="error-message">{error}</span>}
            <Button onClick={login}>login</Button>
        </Surface>
    )

}