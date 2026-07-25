import { signOut } from "@/src/features/auth/actions/sign-out/actions";
import Button from "@/src/shared/components/buttons/Button"
import { Routes } from "@/src/shared/routes";
import { redirect } from "next/navigation";

export type SettingsPageProps = {

}
export default async function SettingsPage({ }: SettingsPageProps) {

    async function logout() {
        "use server";
        await signOut();
        redirect(Routes.public.login, "push");
    }

    return (
        <>
            <h1>settings</h1>
            <div className="flex flex-col gap-2">
                <form action={logout}>
                    <Button type="submit">logout</Button>
                </form>
            </div>
        </>
    )
}