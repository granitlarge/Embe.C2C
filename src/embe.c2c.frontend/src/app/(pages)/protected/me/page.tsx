import { getMe } from "@/src/features/auth/actions/action";
import Me from "@/src/features/me/components/Me";
import MainNav from "@/src/shared/components/nav/MainNav";

export default async function MePage() {

    const getCurrentUserResponse = await getMe();
    if (!getCurrentUserResponse.success) {
        throw new Error("not implemented");
    }

    return (
        <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
            <h1>me</h1>
            <Me className="grow-1 overflow-y-scroll scrollbar-none" user={getCurrentUserResponse.value!} />
            <MainNav className="grow-0" />
        </div>
    )
}