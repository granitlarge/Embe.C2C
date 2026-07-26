"use client";

import Button from "@/src/shared/components/buttons/Button";
import InfoSurface from "@/src/shared/components/infos/InfoSurface";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput"
import Surface from "@/src/shared/components/surfaces/Surface";
import { useState } from "react";
import { sendResetPasswordEmail } from "../actions/action";
import * as z from 'zod';
import { getErrorMessage } from "@/src/shared/error-message";
import { useRouter } from "nextjs-toploader/app";
import { Routes } from "@/src/shared/routes";

export type ForgotPasswordFormProps = {

}
export function ForgotPasswordForm({ }: ForgotPasswordFormProps) {

    const router = useRouter();
    const [email, setEmail] = useState<string | undefined>(undefined);
    const [error, setError] = useState<string | undefined>(undefined);

    async function onSubmit() {

        const validationSchema = z.email("please enter a valid e-mail");
        const validationResult = validationSchema.safeParse(email);
        if (!validationResult.success) {
            const errors = z.treeifyError(validationResult.error).errors;
            setError(errors[0]);
            return;
        }

        const response = await sendResetPasswordEmail(email!);
        if (!response.success) {

            setError(getErrorMessage(response.errors?.[0]));
            return;

        } else {

            router.push(Routes.public.login);

        }
    }

    return (

        <Surface variant="secondary" className="flex flex-col gap-3">
            <InfoSurface show={true}>
                <span className="text-(--primary-fc) text-(length:--primary-fs)">
                    Enter the e-mail address associated with your account. We'll send you an e-mail
                    with instructions on how to reset your password.
                </span>
            </InfoSurface>
            <TextInput
                label="email"
                type="email"
                onBlur={setEmail}
            />
            {error && <span className="mx-auto text-center text-(--error-fc) text-(length:--secondary-fs)">{error}</span>}
            <Button intent="save" onClick={onSubmit}>submit</Button>
        </Surface>

    )

}