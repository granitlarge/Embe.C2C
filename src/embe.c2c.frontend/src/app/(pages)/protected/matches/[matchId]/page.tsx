import { getMatching } from "@/src/features/matches/actions/action";
import Match from "@/src/features/matches/components/Match";
import { FailureReason } from "@/src/shared/api";
import { getAuthenticatedUser } from "@/src/shared/user";
import Link from "next/link";
import { redirect } from "next/navigation";

export type MatchPageProps = {
    params: Promise<{
        matchId: string
    }>
}

export default async function MatchPage({ params }: MatchPageProps) {
    const { matchId } = await params;

    const user = await getAuthenticatedUser();
    const response = await getMatching(matchId);
    if (!response.success) {
        switch (response.reason) {
            case FailureReason.NotFound:
                redirect("public/not-found");
            case FailureReason.Forbidden:
                redirect("public/forbidden");
            case FailureReason.DomainError:
            case FailureReason.Unknown:
            default:
                redirect("public/error");
        }
    }

    const partner = response.value?.user;
    return (
        <div className="flex flex-col h-full">
            <Link href={`/users/${partner?.id!}`} className="no-underline text-(--primary-fc)">
                <h1>{partner?.userName}</h1>
            </Link>
            <Match match={response.value!} user={user!} />
        </div>
    )
}