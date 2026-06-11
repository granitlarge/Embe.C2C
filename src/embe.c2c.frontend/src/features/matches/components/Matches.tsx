import { AuthenticatedUser } from "@/src/shared/user";
import { getMatches } from "../actions/action";
import { MatchCompact } from "./MatchCompact";

export type MatchesProps = {
    user: AuthenticatedUser
};

export async function Matches({ user }: MatchesProps) {
    const result = await getMatches();
    const matches = result.value ?? [];
    return (
        <div className="flex flex-col gap-3 w-full h-full">
            {
                matches.length > 0 ? matches.map(match => (
                    <MatchCompact key={match.id} match={match} user={user} />
                )) : <span className="text-(length:--fs-5) mt-auto mb-auto ml-auto mr-auto">no matches yet</span>
            }
        </div>
    );
}