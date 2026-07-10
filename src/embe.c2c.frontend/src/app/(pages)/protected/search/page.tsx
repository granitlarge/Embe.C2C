import { getCandidates } from "@/src/features/search/actions/action";
import Search from "@/src/features/search/components/Search";
import { getHasSearchProfile } from "@/src/shared/actions/user/action";

export type SearchPageProps = {

}
export default async function SearchPage({ }: SearchPageProps) {

    const getCandidatesResponse = await getCandidates();
    if (!getCandidatesResponse.success) {
        throw new Error("Not implemented");
    }

    let hasSearchProfiles = true;
    if ((getCandidatesResponse.value?.length ?? 0) === 0) {
        const hasSearchProfileResponse = await getHasSearchProfile();
        if (!hasSearchProfileResponse.success) {
            throw new Error("not implemented");
        }
        hasSearchProfiles = hasSearchProfileResponse.value ?? false;
    }

    return (
        <Search
            className="grow-1 overflow-y-scroll scrollbar-none"
            candidates={getCandidatesResponse.value || []}
            hasSearchProfiles={hasSearchProfiles} 
        />
    )
}