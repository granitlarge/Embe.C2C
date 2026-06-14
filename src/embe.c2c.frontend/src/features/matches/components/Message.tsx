import Surface from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { CheckCheck } from "lucide-react";

type MessageContentProps = {
    content: string;
    className?: string;
}
function MessageContent({ content, className }: MessageContentProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ");
    return (
        <span className={`${classNames} whitespace-pre-wrap break-all`}>
            {content}
        </span>
    )
}

export type MessageProps = {
    message: MessageTypeDef;
    isOwn: boolean;
    className?: string;
}
export default function Message({ className, message, isOwn }: MessageProps) {

    const classNames = [
        "max-w-max px-2 py-1",
        isOwn ? "bg-(--message-own-bg)" : "",
        className
    ]
        .filter(Boolean).join(" ");

    const edited = message.editedAt && message.createdAt !== message.editedAt;
    const seen = !!message.seenAt;

    return (
        <Surface className={classNames} padding="none" variant="secondary">
            <div className="flex flex-col gap-0">
                <span className="text-(--secondary-fc) text-(length:--secondary-fs)">{isOwn ? "you" : "them"}</span>
                <MessageContent className="text-(--primary-fc) text-(length:--primary-fs)" content={message.content} />
                <div className="flex gap-2 items-center ml-auto">
                    {
                        seen && isOwn && <CheckCheck size={16} className="text-(--primary-fc)" />
                    }
                    {
                        edited && <span className="text-(length:--secondary-fs) text-(--secondary-fc)">(edited)</span>
                    }
                    <span className="text-(length:--tertiary-fs) text-(--tertiary-fc)">{formatTimeAgo(message.editedAt ?? message.createdAt)}</span>
                </div>
            </div>
        </Surface>
    )

}