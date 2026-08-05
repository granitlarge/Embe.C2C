import { getMe } from "@/src/features/auth/actions/action"
import Settings from "@/src/features/me/components/Settings"
import { SignalRHandlerProvider } from "@/src/shared/providers/signal-r-handler"
import { ApplicationStoreProvider } from "@/src/shared/stores/provider"

export type SettingsPageProps = {

}
export default async function SettingsPage({ }: SettingsPageProps) {
    const user = await getMe();
    if (!user.success || !user.value) {
        throw new Error("not implemented");
    }
    return (
        <ApplicationStoreProvider user={user.value}>
            <SignalRHandlerProvider>
                <Settings />
            </SignalRHandlerProvider>
        </ApplicationStoreProvider>
    )
}