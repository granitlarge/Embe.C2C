import { generateCandidates, getAllSearchProfiles } from "@/src/features/search/actions/action";
import Search from "@/src/features/search/components/Search";
import { getNotifications } from "@/src/shared/actions/notifications/action";
import { getHasSearchProfile } from "@/src/shared/actions/user/action";
import { SignalRProvider } from "@/src/shared/providers/signal-r";
import { ApplicationStoreProvider } from "@/src/shared/stores/provider";

export type SearchPageProps = {

}
export default async function SearchPage({ }: SearchPageProps) {

    const getCandidatesResponsePromise = generateCandidates();
    const getAllSearchProfilesResponsePromise = getAllSearchProfiles(1, 10_000);
    const getNotificationsPromise = getNotifications();

    await Promise.all([getCandidatesResponsePromise, getAllSearchProfilesResponsePromise, getNotificationsPromise]);

    const getCandidatesResponse = await getCandidatesResponsePromise;
    const getAllSearchProfilesResponse = await getAllSearchProfilesResponsePromise;
    const getNotificationsResponse = await getNotificationsPromise;

    if (
        !getCandidatesResponse.success ||
        !getAllSearchProfilesResponse.success ||
        !getNotificationsResponse.success
    ) {
        throw new Error("not implemented");
    }

    let hasSearchProfiles = true;
    if ((getCandidatesResponse.value?.length ?? 0) === 0) {
        const hasSearchProfileResponse = await getHasSearchProfile();
        if (!hasSearchProfileResponse.success) {
            throw new Error("not implemented");
        }
        hasSearchProfiles = hasSearchProfileResponse.value ?? false;
    }

    return (
        <ApplicationStoreProvider
            notifications={getNotificationsResponse.value}
        >
            <SignalRProvider>
                <Search
                    searchProfiles={getAllSearchProfilesResponse.value ?? []}
                    className="grow-1 overflow-y-scroll scrollbar-none"
                    candidates={getCandidatesResponse.value || []}
                    hasSearchProfiles={hasSearchProfiles}
                />
            </SignalRProvider>
        </ApplicationStoreProvider>
    )
}