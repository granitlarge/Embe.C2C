import { getPositiveJudgements } from "@/src/features/likes/actions";
import Likes from "@/src/features/likes/components/Likes";
import MainNav from "@/src/shared/components/nav/MainNav";

export default async function LikesPage() {

    const response = await getPositiveJudgements(1, 50);
    if (!response.success) {
        throw new Error("Failed to fetch positive judgements");
    }

    const positiveJudgements = response.value!;

    return (

        <div className="flex flex-col grow-1 gap-3">

            <h1>likes</h1>

            <Likes className="grow-1" initialLikes={positiveJudgements} />

            <MainNav className="grow-0" />

        </div>

    )

}