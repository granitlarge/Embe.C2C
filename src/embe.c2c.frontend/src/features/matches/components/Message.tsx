import Surface from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { MessagePermission, Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
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
    dto: ReadDto<MessageTypeDef, MessagePermission>;
    isOwn: boolean;
    className?: string;
}
export default function Message({ className, dto, isOwn }: MessageProps) {

    const classNames = [
        "max-w-max px-2 py-1",
        isOwn ? "surface-message" : "",
        className
    ]
        .filter(Boolean).join(" ");

    const message = dto.data;
    const edited = message.editedAt && message.createdAt !== message.editedAt;
    const seen = !!message.seenAt;

    return (
        <Surface className={classNames} padding="none" variant="secondary">
            <div className="flex flex-col gap-0">
                <span className="text-(--secondary-fc) text-(length:--secondary-fs)">{isOwn ? "you" : "them"}</span>
                {message.content && <MessageContent className="text-(--primary-fc) text-(length:--primary-fs)" content={message.content} />}
                <div className="flex gap-2 items-center ml-auto">
                    {
                        seen && isOwn && <CheckCheck className="text-(--tertiary-fc) h-(--primary-fs) w-(--primary-fs)" />
                    }
                    {
                        edited && <span className="text-(length:--tertiary-fs) text-(--tertiary-fc)">(edited)</span>
                    }
                    {(message.editedAt || message.createdAt) && <span className="text-(length:--tertiary-fs) text-(--tertiary-fc)">{formatTimeAgo((message.editedAt ?? message.createdAt)!)}</span>}
                </div>
            </div>
        </Surface>
    )

}