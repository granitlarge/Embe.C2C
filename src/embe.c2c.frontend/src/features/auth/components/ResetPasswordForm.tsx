"use client";

import Button from "@/src/shared/components/buttons/Button";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import Surface from "@/src/shared/components/surfaces/Surface";
import { Routes } from "@/src/shared/routes";
import { useState } from "react";
import * as z from 'zod';
import { resetPassword } from "../actions/action";
import { PasswordValidationRules } from "@/src/shared/validation";
import Link from "@/src/shared/components/Links/Link";

export type ResetPasswordFormProps = {
    onReset: (newPassword: string) => Promise<void> | void;
}
export default function ResetPasswordForm({ onReset }: ResetPasswordFormProps) {

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

            await onReset(password!);

        }

    }

    return (

        <Surface variant="secondary" className="flex flex-col gap-3 items-center justify-center" padding="none">
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
        </Surface>

    )

}