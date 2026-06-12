import Surface, { SurfaceProps } from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { Message } from "@/src/shared/types/domain/aggregates";
import { AuthenticatedUser } from "@/src/shared/user";
import { CheckCheck } from "lucide-react";

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
        <Surface className={`${classNames} flex flex-col justify-center w-full items-center`} padding="none" {...props}>
            <div className="flex flex-row gap-[3px] items-center">
                <span className="max-w-[170px] text-nowrap text-center overflow-hidden text-ellipsis">
                    {
                        message?.content || "no messages"
                    }
                </span>
                {message?.authorUserId !== user.userId && message?.seenAt && <CheckCheck className="text-(--primary)" size={12} />}
            </div>
            {
                message &&
                <span className="text-(length:--fs-secondary) text-(--surface-font-color-muted)">{formatTimeAgo(message?.createdAt)}</span>
            }
        </Surface>
    )
}