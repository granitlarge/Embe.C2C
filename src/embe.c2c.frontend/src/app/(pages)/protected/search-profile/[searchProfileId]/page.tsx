import { getSearchProfile } from "@/src/features/search-profiles/actions";
import SearchProfileForm from "@/src/features/search-profiles/components/SearchProfileForm";
import { Guid } from "@/src/shared/cache";
import MainNav from "@/src/shared/components/nav/MainNav";

export type EditSearchProfilePageProps = {
    params: Promise<{
        searchProfileId: string
    }>
};
export default async function EditSearchProfilePage({ params }: EditSearchProfilePageProps) {
    const { searchProfileId } = await params;
    const getSearchProfileResponse = await getSearchProfile(searchProfileId as Guid);
    if (!getSearchProfileResponse.success) {
        throw new Error("not implemented");
    }
    return (
        <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
            <h1>search-profile</h1>
            <SearchProfileForm searchProfile={getSearchProfileResponse.value?.data} className="grow-1 overflow-y-scroll scrollbar-none" />
            <MainNav />
        </div>
    );
}