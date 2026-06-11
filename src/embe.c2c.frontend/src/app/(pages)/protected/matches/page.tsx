import { Matches } from "@/src/features/matches/components/Matches";
import { getAuthenticatedUser } from "@/src/shared/user";
import { redirect } from "next/navigation";

export default async function MatchesPage() {
  const user = await getAuthenticatedUser();
  if (!user) {
    redirect("/public/login");
  }
  return (
    <Matches user={user} />
  );
}