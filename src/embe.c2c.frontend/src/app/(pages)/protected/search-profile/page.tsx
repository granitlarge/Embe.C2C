import { getAllSearchProfiles } from "@/src/features/search/actions/action";
import SearchProfiles from "@/src/features/search/components/SearchProfiles";

export default async function SearchProfilePage() {

    const getAllSearchProfilesResponse = await getAllSearchProfiles(1, 10);
    if (!getAllSearchProfilesResponse.success) {
        throw new Error("not implemented");
    }

    return (
        <SearchProfiles 
            className="grow-1 overflow-y-scroll scrollbar-none" 
            searchProfiles={getAllSearchProfilesResponse.value ?? []} 
        />
    )
}