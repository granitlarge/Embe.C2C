import RegisterForm from "@/src/features/auth/components/RegisterForm"

export type RegisterPageProps = {

}
export default async function RegisterPage({ }: RegisterPageProps) {

    return (
        <div className="flex justify-start flex-col items-center h-full">
            <h1>register</h1>
            <RegisterForm className="w-[600px] max-w-[100%]" />
        </div>
    )

}