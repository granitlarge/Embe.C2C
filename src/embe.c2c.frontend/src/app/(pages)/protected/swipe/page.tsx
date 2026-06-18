import { getCandidates } from "@/src/features/swipe/actions/action";
import Swipe from "@/src/features/swipe/components/Swipe";
import MainNav from "@/src/shared/components/nav/MainNav";

export type SwipePageProps = {

}
export default async function SwipePage({ }: SwipePageProps) {
    const response = await getCandidates();
    if (!response.success) {
        throw new Error("Not implemented");
    }
    return (
        <div className="flex flex-col grow-1">
            <h1>swipe</h1>
            <Swipe className="grow-1" candidates={response.value || []} />
            <MainNav className="grow-0" />
        </div>
    )
}