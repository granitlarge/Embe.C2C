import Surface from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { Matching, MatchingPermission } from "@/src/shared/types/domain/aggregates";
import { UserCompact } from "./UserCompact";
import Link from "next/link";
import ConversationCompact from "./ConversationCompact";
import { AuthenticatedUser } from "@/src/shared/user";
import { ReadDto } from "@/src/shared/types/dtos/types";

export type MatchCompactProps = {
    dto: ReadDto<Matching, MatchingPermission>;
    className?: string;
    user: AuthenticatedUser;
};

export function MatchCompact({ dto, className, user }: MatchCompactProps) {

    const match = dto.data;
    const otherUser = match.userId1 === user.userId ? match.user2 : match.user1;
    const classNames = [className].filter(Boolean).join(" ");

    return (
        <Surface padding="sm" className={`${classNames} flex flex-row justify-between gap-0`} variant="secondary">
            <UserCompact dto={otherUser} />
            <div className="flex flex-col items-end gap-2 w-full">
                {match.createdAt && <span className="text-(--secondary-fc) text-(length:--secondary-fs) mb-auto">{formatTimeAgo(match.createdAt)}</span>}
                {
                    match.conversation &&
                    <Surface as={Link} className="flex flex-col w-full grow-1 no-underline mb-auto" href={`/protected/matches/${match.id}`} padding="none" variant="inherit">
                        <ConversationCompact className="grow-1 fs-group-secondary" conversation={match.conversation} user={user} />
                    </Surface>
                }
            </div>
        </Surface>
    )

}