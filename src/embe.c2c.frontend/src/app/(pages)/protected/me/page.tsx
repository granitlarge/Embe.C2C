import { getMe } from "@/src/features/auth/actions/action";
import Me from "@/src/features/me/components/Me";
import { getNotifications } from "@/src/shared/actions/notifications/action";
import Link from "@/src/shared/components/Links/Link";
import MainNav from "@/src/shared/components/nav/MainNav";
import { SignalRProvider } from "@/src/shared/providers/signal-r";
import { Routes } from "@/src/shared/routes";
import { ApplicationStoreProvider } from "@/src/shared/stores/provider";
import { Settings } from "@deemlol/next-icons";

export default async function MePage() {

    const getCurrentUserResponsePromise = getMe();
    const getNotificationsResponsePromise = getNotifications();
    const [getCurrentUserResponse, getNotificationsResponse] = await Promise.all([getCurrentUserResponsePromise, getNotificationsResponsePromise]);
    if (
        !getCurrentUserResponse.success ||
        !getCurrentUserResponse.value ||
        !getNotificationsResponse.success ||
        !getNotificationsResponse.value
    ) {
        throw new Error("not implemented");
    }

    return (
        <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
            <ApplicationStoreProvider user={getCurrentUserResponse.value} notifications={getNotificationsResponse.value}>
                <SignalRProvider>

                    <div className="flex justify-between items-center">
                        <h1>me</h1>
                        <Link className="text-(--primary-fc)" href={Routes.protected.settings}>
                            <Settings />
                        </Link>
                    </div>
                    <Me className="grow-1 overflow-y-scroll scrollbar-none" />
                    <MainNav className="grow-0" />

                </SignalRProvider>
            </ApplicationStoreProvider>
        </div>
    )
}