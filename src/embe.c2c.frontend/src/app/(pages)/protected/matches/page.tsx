import { getMatches } from "@/src/features/matches/actions/action";
import { Matches } from "@/src/features/matches/components/Matches";
import { getAuthenticatedUser } from "@/src/shared/user";
import { redirect } from "next/navigation";

export default async function MatchesPage() {

  const user = await getAuthenticatedUser();

  if (!user) {
    redirect("/public/login");
  }

  const response = await getMatches(1, 50);
  const matches = response.value || [];

  return (
    <div className="flex flex-col h-full">
      <h1>matches</h1>
      <Matches className="grow-1" user={user} initialMatches={matches} />
    </div>
  );
}