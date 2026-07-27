"use client";

import Button from "@/src/shared/components/buttons/Button";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import Surface from "@/src/shared/components/surfaces/Surface";
import { Routes } from "@/src/shared/routes";
import { jwtDecode } from "jwt-decode";
import { useState } from "react";
import * as z from 'zod';
import { resetPassword } from "../actions/action";
import { useRouter } from "nextjs-toploader/app";
import { PasswordValidationRules } from "@/src/shared/validation";
import Link from "@/src/shared/components/Links/Link";


export type ResetPasswordFormProps = {
    token: string;
}
export default function ResetPasswordForm({ token }: ResetPasswordFormProps) {

    const router = useRouter();
    const tokenHasExpired = (jwtDecode(token).exp ?? 0) < Date.now() / 1000;

    const [password, setPassword] = useState<string | undefined>(undefined);
    const [confirmPassword, setConfirmPassword] = useState<string | undefined>(undefined);
    const [error, setError] = useState<string | undefined>(undefined);

    async function onSubmit() {

        const validationScheme = z.object({
            password: z.string().min(1, "you must enter a new password"),
            confirmPassword: PasswordValidationRules
        }).refine((data) => data.password === data.confirmPassword, {
            error: "passwords do not match"
        });

        const validationResult = validationScheme.safeParse({ password, confirmPassword });
        if (!validationResult.success) {


            const treeifiedError = z.treeifyError(validationResult.error);
            const properties = treeifiedError.properties;
            const errors = treeifiedError.errors;

            setError(
                properties?.password?.errors[0] ??
                properties?.confirmPassword?.errors[0] ??
                errors[0]
            );

        } else {

            const resetPasswordResponse = await resetPassword(token, password!);
            if (!resetPasswordResponse.success) {
                throw new Error("not implemented");
            }

            router.push(Routes.public.login);

        }

    }

    return (

        <Surface variant="secondary" className="flex flex-col gap-3 items-center justify-center">
            {
                !tokenHasExpired &&
                <>
                    <TextInput
                        label="new password"
                        type="password"
                        onBlur={setPassword}
                    />
                    <TextInput
                        label="confirm new password"
                        type="password"
                        onBlur={setConfirmPassword}
                    />
                    {error && <span className="text-(--error-fc) text-(length:--secondary-fs)">{error}</span>}
                    <Button intent="save" onClick={onSubmit}>submit</Button>
                </>
            }
            {
                tokenHasExpired &&
                <>
                    <span className="text-(--primary-fc) text-(length:--primary-fs) text-center">
                        The password-reset link has expired. Request a new one by clicking <Link href={Routes.public.forgotPassword}>here</Link>.
                    </span>
                </>
            }
        </Surface>

    )

}