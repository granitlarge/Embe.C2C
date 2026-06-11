import Surface, { SurfaceProps } from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { Message } from "@/src/shared/types/domain/aggregates";
import { AuthenticatedUser } from "@/src/shared/user";
import { CheckCheck } from "lucide-react";

function shortenMessage(message: string, maxLength: number) {
    if (message.length <= maxLength) {
        return message;
    }
    return message.slice(0, maxLength) + "...";
}

export type MessageCompactProps = Omit<SurfaceProps<"div">, "as" | "children"> & {
    className?: string;
    message?: Message;
    user: AuthenticatedUser;
}
export default function MessageCompact({ className, message, user, ...props }: MessageCompactProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ");
    return (
        <Surface className={`${classNames} flex flex-col justify-center w-full items-center`} padding="sm" {...props}>
            <div className="flex flex-row gap-[3px] items-center">
                <span className="mt-auto mb-auto text-(length:--fs-lg)">
                    {
                        message?.content && `"${shortenMessage(message?.content, 10)}"` ||
                        "no messages"
                    }
                </span>
                {message?.authorUserId !== user.userId && message?.seenAt && <CheckCheck className="text-(--primary)" size={12} />}
            </div>
            {
                message &&
                <span className="text-(length:--fs-sm) text-(--surface-font-color-muted)">{formatTimeAgo(message?.createdAt)}</span>
            }
        </Surface>
    )
}