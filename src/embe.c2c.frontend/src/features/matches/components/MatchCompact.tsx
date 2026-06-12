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
        <Surface padding="sm" className={`${classNames} flex flex-row justify-between gap-5`}>
            <UserCompact className="" userBrief={otherUser} />
            <div className="flex flex-col items-end gap-2 w-full">
                <span className="text-(length:--fs-secondary) text-(--surface-font-color-muted)">{formatTimeAgo(match.createdAt)}</span>
                <Surface as={Link} className="w-full no-underline" href={`/protected/matches/${match.id}`} padding="none">
                    <ConversationCompact className="bg-(--surface-light)" conversation={match.conversation} user={user} />
                </Surface>
            </div>

        </Surface>
    )

}