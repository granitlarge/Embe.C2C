import Surface from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { Matching, MatchingPermission } from "@/src/shared/types/domain/aggregates";
import { UserCompact } from "./UserCompact";
import Link from "next/link";
import { AuthenticatedUser } from "@/src/shared/user";
import { ReadDto } from "@/src/shared/types/dtos/types";
import MessageCompact from "./MessageBrief";
import { Routes } from "@/src/shared/routes";

export type MatchCompactProps = {
    dto: ReadDto<Matching, MatchingPermission>;
    className?: string;
    user: AuthenticatedUser;
};

export function MatchCompact({ dto, className, user }: MatchCompactProps) {

    const match = dto.data;
    const otherUser = match.userId1 === user.userId ? match.user2 : match.user1;
    const searchProfile = match.userId1 === user.userId ? match.user1SearchProfile : match.user2SearchProfile;
    const otherSearchProfile = match.userId1 === user.userId ? match.user2SearchProfile : match.user1SearchProfile;
    const classNames = [className].filter(Boolean).join(" ");

    return (
        <Surface padding="sm" className={`${classNames} flex flex-col`} variant="secondary">
            {searchProfile && <span className="mx-auto text-(--primary-fc) text-(length:--primary-fs) font-bold" >{searchProfile?.data.name}</span>}
            <div className="flex flex-row justify-between gap-3 w-full">
                <UserCompact dto={otherUser} />
                <div className="flex flex-col items-end gap-2 w-full">
                    {match.createdAt && <span className="text-(--secondary-fc) text-(length:--secondary-fs) mb-auto" suppressHydrationWarning>{formatTimeAgo(match.createdAt)}</span>}
                    {
                        <Surface as={Link} className="flex flex-col w-full grow-1 no-underline mb-auto" href={Routes.protected.match(match.id)} padding="none" variant="inherit">
                            <Surface className={`grow-1 fs-group-primary w-full flex flex-col justify-center`} padding="none" variant="inherit">
                                {
                                    dto.data.lastMessage && <MessageCompact className="grow-1" messageDto={dto.data.lastMessage} user={user} /> ||
                                    <span className="surface-tertiary text-(--primary-fc) text-center text-(length:--primary-fs) w-full grow-1 flex items-center justify-center rounded-md">
                                        no messages yet
                                    </span>
                                }
                            </Surface>
                        </Surface>
                    }
                </div>
            </div>
        </Surface>
    )

}