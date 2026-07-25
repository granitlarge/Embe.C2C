import { generateCandidates, getAllSearchProfiles } from "@/src/features/search/actions/action";
import Search from "@/src/features/search/components/Search";
import { getHasSearchProfile } from "@/src/shared/actions/user/action";
import { getNextConfigRuntime } from "next/dist/server/config-shared";

export type SearchPageProps = {

}
export default async function SearchPage({ }: SearchPageProps) {

    const getCandidatesResponsePromise = generateCandidates();
    const getAllSearchProfilesResponsePromise = getAllSearchProfiles(1, 10_000);
    await Promise.all([getCandidatesResponsePromise, getAllSearchProfilesResponsePromise]);
    const getCandidatesResponse = await getCandidatesResponsePromise;
    const getAllSearchProfilesResponse = await getAllSearchProfilesResponsePromise;

    if (!getCandidatesResponse.success || !getAllSearchProfilesResponse.success) {
        throw new Error("not implemented");
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
            searchProfiles={getAllSearchProfilesResponse.value ?? []}
            className="grow-1 overflow-y-scroll scrollbar-none"
            candidates={getCandidatesResponse.value || []}
            hasSearchProfiles={hasSearchProfiles} 
        />
    )
}