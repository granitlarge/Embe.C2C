import { getMatching } from "@/src/features/matches/actions/action";
import Match from "@/src/features/matches/components/Match";
import { FailureReason } from "@/src/shared/apis/type";
import { Guid } from "@/src/shared/cache";
import { getAuthenticatedUser } from "@/src/shared/user";
import { redirect } from "next/navigation";

export type MatchPageProps = {
    params: Promise<{
        matchId: Guid
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

    const matchDto = response.value;
    const match = matchDto?.data;

    return (
        <div className="flex flex-col h-full">
            <Match className="grow-1 overflow-scroll scrollbar-none" match={response.value!} user={user!} />
        </div>
    )

}