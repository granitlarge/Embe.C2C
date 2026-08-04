import Button from "@/src/shared/components/buttons/Button"
import ErrorMessage from "@/src/shared/components/inputs/ErrorMessage"
import TextInput from "@/src/shared/components/inputs/text-input/TextInput"
import Surface from "@/src/shared/components/surfaces/Surface"
import { useState } from "react"
import * as z from 'zod';
import { sendVerificationEmail } from "../../auth/actions/action"
import * as api from '../actions/action'

export type ChangeEmailFormProps = {

}
export default function ChangeEmailForm({ }: ChangeEmailFormProps) {

    const [step, setStep] = useState<"email" | "code" | "success">("email");

    const [newEmail, setNewEmail] = useState<string | undefined>(undefined);
    const [newEmailError, setNewEmailError] = useState<string | undefined>(undefined);

    const [verificationCode, setVerificationCode] = useState<string | undefined>(undefined);
    const [verificationCodeError, setVerificationCodeError] = useState<string | undefined>(undefined);

    async function onEmailSubmit() {

        const validationSchema = z.email().min(1, { error: "please enter a valid e-amil address" });
        const validationResult = validationSchema.safeParse(newEmail);
        if (!validationResult.success) {
            setNewEmailError(validationResult.error.issues[0].message);
            return;
        }

        const sendVerificationEmailResponse = await sendVerificationEmail(newEmail!);
        if (!sendVerificationEmailResponse.success) {
            throw new Error("not implemeneted");
        }

        setStep("code");

    }

    async function onVerificationCodeSubmit() {

        const validationSchema = z.string().min(1, { error: "please enter the verification code to continue" });
        const validationResult = validationSchema.safeParse(verificationCode);

        if (!validationResult.success) {
            setVerificationCodeError(validationResult.error.issues[0].message);
            return;
        }

        const changeEmailResult = await api.changeEmail(newEmail!, verificationCode!);
        if (!changeEmailResult.success) {
            const hasDuplicatedEmailError = changeEmailResult.errors?.find(e => e.code === "auth.duplicate_email");
            if (hasDuplicatedEmailError) {
                setNewEmailError("that e-mail is already associated with another account");
                setStep("email");
            } else {
                setVerificationCodeError("an unknown error occurred");
            }
            return;
        }

        setStep("success");

    }

    return (
        <Surface className="flex flex-col gap-3" variant="inherit">
            {
                step === "email" &&
                <>
                    <TextInput
                        placeholder="new email"
                        onBlur={setNewEmail}
                        value={newEmail}
                    />
                    <ErrorMessage message={newEmailError} />
                    <Button intent="navigate" onClick={onEmailSubmit}>
                        next
                    </Button>
                </>
            }
            {
                step === "code" &&
                <>
                    <p className="text-center text-(--primary-fc) text-(length:--primary-fs)">
                        A verification code has been sent to <strong>{newEmail}</strong>, enter it below to change your email.
                    </p>
                    <TextInput
                        placeholder="verification code"
                        onBlur={setVerificationCode}
                    />
                    <ErrorMessage message={verificationCodeError} />
                    <Button intent="save" onClick={onVerificationCodeSubmit}>
                        change email
                    </Button>
                </>
            }
            {
                step === "success" &&
                <>
                    <p className="text-center text-(--primary-fc) text-(length:--primary-fs)">
                        Your e-mail has been changed to <strong>{newEmail}</strong>.
                    </p>
                </>
            }
        </Surface>
    )

}