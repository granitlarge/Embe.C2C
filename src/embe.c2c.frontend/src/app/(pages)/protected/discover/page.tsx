import { getNotifications } from "@/src/shared/actions/notifications/action";
import MainNav from "@/src/shared/components/nav/MainNav"
import { SignalRProvider } from "@/src/shared/providers/signal-r"
import { SignalRHandlerProvider } from "@/src/shared/providers/signal-r-handler";
import { ApplicationStoreProvider } from "@/src/shared/stores/provider"

export type DiscoverPageProps = {

}
export default async function DiscoverPage({ }: DiscoverPageProps) {

    const getNotificationsPromise = getNotifications();

    await Promise.all([getNotificationsPromise]);

    const getNotificationsResponse = await getNotificationsPromise;

    if (!getNotificationsResponse.success || !getNotificationsResponse.value) {
        throw new Error("not implemented");
    }

    return (
        <ApplicationStoreProvider
            notifications={getNotificationsResponse.value}
        >
            <SignalRHandlerProvider>
                <div className="flex flex-col grow-1 gap-3">
                    <h1>discover</h1>
                    <div className="grow-1">
                    </div>
                    <MainNav className="grow-0" />
                </div>
            </SignalRHandlerProvider>
        </ApplicationStoreProvider>
    )
}