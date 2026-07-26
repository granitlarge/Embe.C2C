"use client";

import Button from "@/src/shared/components/buttons/Button";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import { useState } from "react";
import * as z from "zod";
import { signIn } from "../actions/sign-in/actions";
import Surface from "@/src/shared/components/surfaces/Surface";
import Link from "@/src/shared/components/Links/Link";
import { useRouter } from "nextjs-toploader/app";
import { Routes } from "@/src/shared/routes";
import { getErrorMessage } from "@/src/shared/error-message";

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
            if (error?.[0] !== undefined) {

                console.log(error);
                setError(getErrorMessage(error[0]))

            } else {

                router.refresh();
                router.replace(Routes.protected.search);

            }

        }

    }

    function clearErrors() {
        setUsernameError(undefined);
        setPasswordError(undefined);
        setError(undefined);
    }
    const passwordLabel = <>password<Link href={Routes.public.forgotPassword} title="Forgot Password?">?</Link></>;
    return (
        <Surface className={`form w-[600px] max-w-full ${classNames} relative`} variant="secondary">
            <TextInput
                label="email"
                type="email"
                placeholder="name@example.com"
                value={userName}
                errorMessage={usernameError}
                onBlur={(un: string) => {
                    setUsername(un);
                    clearErrors();
                }} />
            <TextInput
                label={passwordLabel} type="password" placeholder="***********" value={password} errorMessage={passwordError} onBlur={(pw: string) => {
                    setPassword(pw);
                    clearErrors();
                }} />
            {error && <span className="text-center mx-auto text-(--error-fc) text-(length:--secondary-fs)">{error}</span>}
            <Button onClick={login} intent="save">login</Button>
            <Link className="absolute right-2 top-2" href={Routes.public.register}>register</Link>
        </Surface>
    )

}