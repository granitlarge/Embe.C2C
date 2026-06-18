import Surface from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { MessagePermission, Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { CheckCheck, Edit, Flag, Reply, Trash2 } from "lucide-react";

type MessageContentProps = {
    content: string;
    className?: string;
}
function MessageContent({ content, className }: MessageContentProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ");
    return (
        <span className={`${classNames} whitespace-pre-wrap break-word`}>
            {content}
        </span>
    )
}

export type MessageProps = {
    dto: ReadDto<MessageTypeDef, MessagePermission>;
    className?: string;
    onEdit?: () => void;
    onDelete?: () => void;
    onReport?: () => void;
    onReply?: () => void;
    isOwn: boolean
}
export default function Message({ className, dto, onEdit, onDelete, onReport, onReply, isOwn }: MessageProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const canEdit = dto.permissions.some(p => p === MessagePermission.Edit);
    const canDelete = dto.permissions.some(p => p === MessagePermission.Delete);
    const canReport = dto.permissions.some(p => p === MessagePermission.Report);
    const canReply = dto.permissions.some(p => p === MessagePermission.Reply);

    const message = dto.data;
    const edited = message.editedAt && message.createdAt !== message.editedAt;
    const seen = !!message.seenAt;

    const actionIconClassNames = "text-(--secondary-fc) h-(--secondary-fs) w-(--secondary-fs)";

    return (
        <Surface className={`${classNames} w-full px-2 py-1`} padding="none" variant="none">
            <div className="flex flex-col gap-1">
                <div className="flex gap-2 justify-between items-center">
                    <span className="text-(--secondary-fc) text-(length:--secondary-fs)">{isOwn ? "you" : "them"}</span>
                    <div className="flex gap-1 items-center">
                        {!isOwn && canReply && onReply && <button className="p-0 bg-transparent" onClick={onReply}><Reply className={actionIconClassNames} /></button>}
                        {canEdit && onEdit && <button className="p-0 bg-transparent" onClick={onEdit}><Edit className={actionIconClassNames} /></button>}
                        {canDelete && onDelete && <button className="p-0 bg-transparent" onClick={onDelete}><Trash2 className={actionIconClassNames} /></button>}
                        {canReport && onReport && <button className="p-0 bg-transparent" onClick={onReport}><Flag className={actionIconClassNames} /></button>}
                    </div>
                </div>
                {message.content && <MessageContent className={`text-(--primary-fc) text-(length:--primary-fs) ${isOwn ? "ml-auto" : "mr-auto"}`} content={message.content} />}
                <div className="flex gap-2 items-center ml-auto">
                    {
                        seen && isOwn && <CheckCheck className="text-(--tertiary-fc) h-(--primary-fs) w-(--primary-fs)" />
                    }
                    {
                        edited && <span className="text-(length:--tertiary-fs) text-(--tertiary-fc)">(edited)</span>
                    }
                    {(message.editedAt || message.createdAt) && <span suppressHydrationWarning className="text-(length:--tertiary-fs) text-(--tertiary-fc)">{formatTimeAgo((message.editedAt ?? message.createdAt)!)}</span>}
                </div>
            </div>
        </Surface>
    )

}