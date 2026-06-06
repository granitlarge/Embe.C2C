import LoginForm from "@/src/features/auth/components/LoginForm"

export type LoginPageProps = {

}
export default async function LoginPage({ }: LoginPageProps) {
    return (
        <div className="flex flex-col items-center justify-start">
            <h1>login</h1>
            <LoginForm />
        </div>
    )
}