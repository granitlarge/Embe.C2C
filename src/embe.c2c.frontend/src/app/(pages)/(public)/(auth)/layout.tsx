import { getAuthenticatedUser } from "@/src/shared/user"
import { redirect } from "next/navigation";

export default async function AuthLayout({
    children,
}: { children: React.ReactNode }) {

    const user = await getAuthenticatedUser()
    if (user) {
        redirect("/");
    }

    return (
        { children }
    )
}