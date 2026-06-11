import Surface from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { Matching } from "@/src/shared/types/domain/aggregates";
import { UserCompact } from "./UserCompact";
import Link from "next/link";
import ConversationCompact from "./ConversationCompact";
import { AuthenticatedUser } from "@/src/shared/user";

export type MatchCompactProps = {
    match: Matching;
    className?: string;
    user: AuthenticatedUser;
};

export async function MatchCompact({ match, className, user }: MatchCompactProps) {

    const otherUser = match.user;
    const classNames = [className].filter(Boolean).join(" ");

    return (
        <Surface padding="sm" className={`${classNames} flex flex-row justify-between gap-1`}>
            <UserCompact userBrief={otherUser} />
            <Surface as={Link} className="no-underline flex flex-col justify-center items-end grow-1 gap-1" href={`/protected/matches/${match.id}`} padding="none">
                <span className="text-(length:--fs-md) text-(--surface-font-color-muted)">{formatTimeAgo(match.createdAt)}</span>
                <ConversationCompact className="bg-(--surface-light)" conversation={match.conversation} user={user} />
            </Surface>
        </Surface>
    )

}