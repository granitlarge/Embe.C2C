import { getCandidates } from "@/src/features/swipe/actions/action";
import Swipe from "@/src/features/swipe/components/Swipe";

export type SwipePageProps = {

}
export default async function SwipePage({ }: SwipePageProps) {
    const response = await getCandidates();
    if (!response.success) {
        throw new Error("Not implemented");
    }
    return (
        <div>
            <h1>swipe</h1>
            <Swipe candidates={response.value || []} />
        </div>
    )
}