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

export function MatchCompact({ match, className, user }: MatchCompactProps) {

    const otherUser = match.user;
    const classNames = [className].filter(Boolean).join(" ");

    return (
        <Surface padding="sm" className={`${classNames} flex flex-row justify-between gap-0`} variant="secondary">
            <UserCompact userBrief={otherUser} />
            <div className="flex flex-col items-end gap-2 w-full">
                <span className="text-(--secondary-fc) text-(length:--secondary-fs) mb-auto">{formatTimeAgo(match.createdAt)}</span>
                <Surface as={Link} className="flex flex-col w-full grow-1 no-underline mb-auto" href={`/protected/matches/${match.id}`} padding="none" variant="inherit">
                    <ConversationCompact className="grow-1 fs-group-secondary" conversation={match.conversation} user={user} />
                </Surface>
            </div>
        </Surface>
    )

}