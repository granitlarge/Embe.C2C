import { getMatchings } from "@/src/features/matches/actions/action";
import { Matches } from "@/src/features/matches/components/Matches";
import { getNotifications } from "@/src/shared/actions/notifications/action";
import MainNav from "@/src/shared/components/nav/MainNav";
import { SignalRProvider } from "@/src/shared/providers/signal-r";
import { SignalRHandlerProvider } from "@/src/shared/providers/signal-r-handler";
import { Routes } from "@/src/shared/routes";
import { ApplicationStoreProvider } from "@/src/shared/stores/provider";
import { getAuthenticatedUser } from "@/src/shared/user";
import { redirect } from "next/navigation";

export default async function MatchesPage() {

  const user = await getAuthenticatedUser();

  if (!user) {
    redirect(Routes.public.login);
  }

  const getMatchingsPromise = getMatchings(1, 50);
  const getNotificationsPromise = getNotifications();

  await Promise.all([getMatchingsPromise, getNotificationsPromise]);

  const getMatchingsResponse = await getMatchingsPromise;
  const getNotificationsResponse = await getNotificationsPromise;

  return (
    <div className="flex flex-col gap-3 grow-1">
      <ApplicationStoreProvider matchings={getMatchingsResponse.value} user={undefined} notifications={getNotificationsResponse.value}>
        <SignalRHandlerProvider>

          <h1>matches</h1>
          <Matches className="grow-1" user={user}  />
          <MainNav className="grow-0" />

        </SignalRHandlerProvider>
      </ApplicationStoreProvider>
    </div>
  );
}