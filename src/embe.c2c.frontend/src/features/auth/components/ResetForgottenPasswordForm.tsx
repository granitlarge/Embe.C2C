"use client";

import { jwtDecode } from "jwt-decode";
import { useRouter } from "nextjs-toploader/app";
import ResetPasswordForm from "./ResetPasswordForm";
import Link from "@/src/shared/components/Links/Link";
import { Routes } from "@/src/shared/routes";
import Surface from "@/src/shared/components/surfaces/Surface";
import * as api from "../actions/action";

export type ResetForgottenPasswordFormProps = {
    token: string;
}
export default function ResetForgottenPasswordForm({ token }: ResetForgottenPasswordFormProps) {

    const router = useRouter();
    const tokenHasExpired = (jwtDecode(token).exp ?? 0) < Date.now() / 1000;

    async function onReset(newPassword: string) {
        const response = await api.resetPassword(token, newPassword);
        if (!response.success) {
            throw new Error("not implemented");
        }
        router.push(Routes.public.login);
    }

    return (
        <Surface className="flex flex-col gap-3 items-center justify-center" variant="secondary" padding="sm">
            {
                tokenHasExpired &&
                <>
                    <span className="text-(--primary-fc) text-(length:--primary-fs) text-center">
                        The password-reset link has expired. Request a new one by clicking <Link href={Routes.public.forgotPassword}>here</Link>.
                    </span>
                </>
            }
            {
                !tokenHasExpired &&
                <ResetPasswordForm onReset={onReset} />
            }
        </Surface>
    )

}