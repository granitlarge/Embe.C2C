import { ForgotPasswordForm } from "@/src/features/auth/components/ForgotPasswordForm"

export type ForgotPasswordProps = {

}
export default async function ForgotPassword({ }: ForgotPasswordProps) {
    return (
        <div className="flex flex-col gap-3">
            <h1>forgot password</h1>
            <ForgotPasswordForm />
        </div>
    )
}