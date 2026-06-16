import Surface, { SurfaceProps, Variant } from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { Message, MessagePermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { AuthenticatedUser } from "@/src/shared/user";
import { CheckCheck } from "lucide-react";

export type MessageCompactProps = Omit<SurfaceProps<"div">, "as" | "children"> & {
    className?: string;
    messageDto: ReadDto<Message, MessagePermission>;
    user: AuthenticatedUser;
}

export default function MessageCompact({ className, messageDto, user, ...props }: MessageCompactProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const message = messageDto.data;
    const isOwn = message.authorUserId === user.userId;
    const isSeen = !!message.seenAt;

    return (
        <Surface className={`${classNames} flex flex-col justify-center w-full items-center`} padding="none" {...props} variant="tertiary">
            <div className="flex flex-row gap-[3px] items-center">
                {isOwn && isSeen && <CheckCheck className="text-(--universal-primary-bg)" size={12} />}
                {!isOwn && !isSeen && <span className="text-(--universal-primary-bg)">•</span>}
                <span className="max-w-[170px] text-nowrap text-center overflow-hidden text-ellipsis text-(length:--primary-fs)">
                    {isOwn ? <span className="text-(--secondary-fc) text-(length:--secondary-fs)">you: </span> : <span className="text-(--secondary-fc) text-(length:--secondary-fs)">them: </span>} {message.content}
                </span>
            </div>
            {
                message && message.createdAt &&
                <span className="text-(--secondary-fc) text-(length:--secondary-fs)">{formatTimeAgo(message.createdAt)}</span>
            }
        </Surface>
    )
}