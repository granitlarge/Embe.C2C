import { getPositiveJudgements } from "@/src/features/likes/actions";
import Likes from "@/src/features/likes/components/Likes";
import { getNotifications } from "@/src/shared/actions/notifications/action";
import MainNav from "@/src/shared/components/nav/MainNav";
import { SignalRProvider } from "@/src/shared/providers/signal-r";
import { ApplicationStoreProvider } from "@/src/shared/stores/provider";

export default async function LikesPage() {

    const getPositiveJudgementsPromise = getPositiveJudgements(1, 50);
    const getNotificationsPromise = getNotifications();

    await Promise.all([getPositiveJudgementsPromise, getNotificationsPromise]);
    const getNotificationsResponse = await getNotificationsPromise;
    const getPositiveJudgementsResponse = await getPositiveJudgementsPromise;

    if (
        !getPositiveJudgementsResponse.success || !getPositiveJudgementsResponse.value ||
        !getNotificationsResponse.success || !getNotificationsResponse.value
    ) {
        throw new Error("not implemented");
    }

    return (

        <ApplicationStoreProvider
            notifications={getNotificationsResponse.value}
            positiveJudgements={getPositiveJudgementsResponse.value}
        >
            <SignalRProvider>

                <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">

                    <h1>likes</h1>
                    <Likes className="grow-1 overflow-y-scroll scrollbar-none" />
                    <MainNav className="grow-0" />

                </div>

            </SignalRProvider>
        </ApplicationStoreProvider>

    )

}