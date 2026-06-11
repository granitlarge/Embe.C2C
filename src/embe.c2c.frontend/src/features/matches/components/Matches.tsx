import { AuthenticatedUser } from "@/src/shared/user";
import { getMatches } from "../actions/action";
import { MatchCompact } from "./MatchCompact";

export type MatchesProps = {
    user: AuthenticatedUser
    className?: string;
};

export async function Matches({ className, user }: MatchesProps) {
    const result = await getMatches();
    const matches = result.value || [];
    const classNames = [className].filter(Boolean).join(" ");
    return (
        <div className={`flex flex-col gap-3 ${classNames}`}>
            {
                matches.length > 0 ? matches.map(match => (
                    <MatchCompact key={match.id} match={match} user={user} />
                )) : <span className="text-(length:--fs-5) mx-auto my-auto">no matches yet</span>
            }
        </div>
    );
}