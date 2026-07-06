import { getCandidates } from "@/src/features/find/actions/action";
import Find from "@/src/features/find/components/Find";
import MainNav from "@/src/shared/components/nav/MainNav";

export type FindPageProps = {

}
export default async function FindPage({ }: FindPageProps) {
    const getCandidatesResponse = await getCandidates();
    if (!getCandidatesResponse.success) {
        throw new Error("Not implemented");
    }
    return (
        <div className="flex flex-col grow-1 gap-3">
            <h1>find</h1>
            <Find className="grow-1" candidates={getCandidatesResponse.value || []} />
            <MainNav className="grow-0" />
        </div>
    )
}