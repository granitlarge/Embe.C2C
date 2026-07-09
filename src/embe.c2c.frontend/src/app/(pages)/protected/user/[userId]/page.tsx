import { getProfile } from "@/src/shared/actions/user/action";
import { Guid } from "@/src/shared/cache";
import MainNav from "@/src/shared/components/nav/MainNav";
import Profile from "@/src/shared/components/profiles/Profile";
import Surface from "@/src/shared/components/surfaces/Surface";

export type ProfilePageProps = {
    params: Promise<{
        userId: string;
    }>;
};
export default async function ProfilePage({ params }: ProfilePageProps) {

    const { userId } = await params;
    const getProfileResponse = await getProfile(userId as Guid);
    if (!getProfileResponse.success) {
        throw new Error("not implemented");
    }

    return (
        <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none" >
            <h1>{getProfileResponse.value?.data.alias}</h1>
            <Profile className="grow-1 overflow-y-scroll scrollbar-none" user={getProfileResponse.value?.data!} />
            <MainNav className="grow-0" />
        </div>
    );

}