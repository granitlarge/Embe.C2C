import { getCandidates } from "@/src/features/find/actions/action";
import Find from "@/src/features/find/components/Find";
import { getHasSearchProfile } from "@/src/shared/actions/user/action";
import MainNav from "@/src/shared/components/nav/MainNav";

export type FindPageProps = {

}
export default async function FindPage({ }: FindPageProps) {

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
        <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
            <h1>find</h1>
            <Find className="grow-1 overflow-y-scroll scrollbar-none" hasSearchProfiles={hasSearchProfiles} candidates={getCandidatesResponse.value || []} />
            <MainNav className="grow-0" />
        </div>
    )
}