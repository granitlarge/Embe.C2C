import { getMatchings } from "@/src/features/matches/actions/action";
import { Matches } from "@/src/features/matches/components/Matches";
import MainNav from "@/src/shared/components/nav/MainNav";
import { Routes } from "@/src/shared/routes";
import { getAuthenticatedUser } from "@/src/shared/user";
import { redirect } from "next/navigation";

export default async function MatchesPage() {

  const user = await getAuthenticatedUser();

  if (!user) {
    redirect(Routes.public.login);
  }

  const response = await getMatchings(1, 50);
  const matches = response.value || [];

  return (
    <div className="flex flex-col gap-3 grow-1">
      <h1>matches</h1>
      <Matches className="grow-1" user={user} initialMatches={matches} />
      <MainNav className="grow-0"/>
    </div>
  );
}