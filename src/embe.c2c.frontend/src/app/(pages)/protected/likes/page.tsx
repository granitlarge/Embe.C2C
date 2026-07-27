import { getPositiveJudgements } from "@/src/features/likes/actions";
import Likes from "@/src/features/likes/components/Likes";
import MainNav from "@/src/shared/components/nav/MainNav";
import { SignalRProvider } from "@/src/shared/providers/signal-r";
import { ApplicationStoreProvider } from "@/src/shared/stores/provider";

export default async function LikesPage() {

    const response = await getPositiveJudgements(1, 50);
    if (!response.success) {
        throw new Error("Failed to fetch positive judgements");
    }

    const positiveJudgements = response.value!;

    return (

        <ApplicationStoreProvider>
            <SignalRProvider>
                <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">

                    <h1>likes</h1>

                    <Likes className="grow-1 overflow-y-scroll scrollbar-none" initialLikes={positiveJudgements} />

                    <MainNav className="grow-0" />

                </div>
            </SignalRProvider>
        </ApplicationStoreProvider>

    )

}