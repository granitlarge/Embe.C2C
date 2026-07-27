import { getMe } from "@/src/features/auth/actions/action";
import { getSearchProfile } from "@/src/features/search-profiles/actions";
import SearchProfileForm from "@/src/features/search-profiles/components/SearchProfileForm";
import { Guid } from "@/src/shared/cache";
import BackButton from "@/src/shared/components/buttons/BackButton";

export type EditSearchProfilePageProps = {
    params: Promise<{
        searchProfileId: string
    }>
};
export default async function EditSearchProfilePage({ params }: EditSearchProfilePageProps) {
    const { searchProfileId } = await params;
    const getSearchProfileResponse = await getSearchProfile(searchProfileId as Guid);
    const getMeResponse = await getMe();
    if (
        !getSearchProfileResponse.success ||
        !getSearchProfileResponse.value?.data ||
        !getMeResponse.success ||
        !getMeResponse.value?.data
    ) {
        throw new Error("not implemented");
    }
    return (
        <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
            <div className="flex flex-row justify-between">
                <div className="flex flex-row gap-3 items-center justify-center">
                    <BackButton />
                </div>
                <h1>search-profile</h1>
            </div>
            <SearchProfileForm
                user={getMeResponse.value.data}
                searchProfile={getSearchProfileResponse.value.data}
                className="grow-1 overflow-y-scroll scrollbar-none"
            />
        </div>
    );
}