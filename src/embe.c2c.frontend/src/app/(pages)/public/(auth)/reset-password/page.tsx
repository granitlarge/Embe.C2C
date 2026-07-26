import ResetPasswordForm from "@/src/features/auth/components/ResetPasswordForm"
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

            <ResetPasswordForm token={token as string} />

        </div>

    )

}