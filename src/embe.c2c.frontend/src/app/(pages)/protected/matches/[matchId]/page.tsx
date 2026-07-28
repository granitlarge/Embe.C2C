import { getMatching } from "@/src/features/matches/actions/action";
import Match from "@/src/features/matches/components/Match";
import { getNotifications } from "@/src/shared/actions/notifications/action";
import { Guid } from "@/src/shared/cache";
import { SignalRProvider } from "@/src/shared/providers/signal-r";
import { SignalRHandlerProvider } from "@/src/shared/providers/signal-r-handler";
import { ApplicationStoreProvider } from "@/src/shared/stores/provider";
import { getAuthenticatedUser } from "@/src/shared/user";

export type MatchPageProps = {
    params: Promise<{
        matchId: Guid
    }>
}
export default async function MatchPage({ params }: MatchPageProps) {

    const { matchId } = await params;

    const user = await getAuthenticatedUser();
    const getNotificationsPromise = getNotifications();
    const getMatchingsPromise = getMatching(matchId);
    await Promise.all([getNotificationsPromise, getMatchingsPromise]);

    const getNotificationsResponse = await getNotificationsPromise;
    const getMatchingResponse = await getMatchingsPromise;

    if (
        !getNotificationsResponse.success || !getNotificationsResponse.value ||
        !getMatchingResponse.success || !getMatchingResponse.value
    ) {
        throw new Error("not implemented");
    }

    const matchDto = getMatchingResponse.value;
    const match = matchDto?.data;

    return (
        <ApplicationStoreProvider matchings={[getMatchingResponse.value]} notifications={getNotificationsResponse.value}>
            <SignalRHandlerProvider>
                <div className="flex flex-col h-full">
                    <Match className="grow-1 overflow-scroll scrollbar-none" matchId={matchId} user={user!} />
                </div>
            </SignalRHandlerProvider>
        </ApplicationStoreProvider>
    )

}