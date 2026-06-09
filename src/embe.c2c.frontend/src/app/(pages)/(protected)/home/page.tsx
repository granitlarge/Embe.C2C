import { getAuthenticatedUser } from "@/src/shared/user";
import { redirect } from "next/navigation";

export default async function HomePage() {

    const user = await getAuthenticatedUser();

    if (!user) {
        redirect("/login");
    }

    return (
        <div>
            <h1>Home</h1>
        </div>
    );

}