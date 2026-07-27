import MainNav from "@/src/shared/components/nav/MainNav"
import { SignalRProvider } from "@/src/shared/providers/signal-r"
import { ApplicationStoreProvider } from "@/src/shared/stores/provider"

export type DiscoverPageProps = {

}
export default async function DiscoverPage({ }: DiscoverPageProps) {
    return (
        <ApplicationStoreProvider>
            <SignalRProvider>
                <div className="flex flex-col grow-1 gap-3">
                    <h1>discover</h1>
                    <div className="grow-1">
                    </div>
                    <MainNav className="grow-0" />
                </div>
            </SignalRProvider>
        </ApplicationStoreProvider>
    )
}