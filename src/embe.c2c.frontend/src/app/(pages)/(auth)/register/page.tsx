import RegisterForm from "@/src/features/auth/components/RegisterFlow"

export type RegisterPageProps = {

}

export default async function RegisterPage({ }: RegisterPageProps) {
    return (
        <div className="w-full h-full flex justify-center">
            <div className="flex flex-col gap-6 items-center w-full">
                <h1>register</h1>
                <RegisterForm className="w-[600px] h-[600px]-6"/>
            </div>
        </div>
    )
}