import ResetForgottenPasswordForm from "@/src/features/auth/components/ResetForgottenPasswordForm";
import { Routes } from "@/src/shared/routes";
import { redirect } from "next/navigation"

export type ResetPasswordPageProps = {
    searchParams: Promise<{ token: string }>
}

export default async function ResetPasswordPage({ searchParams }: ResetPasswordPageProps) {

    const { token } = await searchParams;

    if (!token) {
        redirect(Routes.public.login);
    }

    return (

        <div className="flex flex-col gap-3">

            <h1>reset password</h1>

            <ResetForgottenPasswordForm token={token as string} />

        </div>

    )

}