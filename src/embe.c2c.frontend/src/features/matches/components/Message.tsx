import Surface from "@/src/shared/components/surfaces/Surface";
import { formatTimeAgo } from "@/src/shared/time";
import { MessagePermission, Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { CheckCheck, Edit, Flag, Trash2 } from "lucide-react";

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
    onEdit?: () => void;
    onDelete?: () => void;
    onReport?: () => void;
}
export default function Message({ className, dto, isOwn, onEdit, onDelete, onReport }: MessageProps) {

    const classNames = [
        "max-w-max px-2 py-1",
        isOwn ? "surface-message" : "surface-secondary",
        className
    ].filter(Boolean).join(" ");

    const canEdit = dto.permissions.some(p => p === MessagePermission.Edit);
    const canDelete = dto.permissions.some(p => p === MessagePermission.Delete);
    const canReport = dto.permissions.some(p => p === MessagePermission.Report);

    const message = dto.data;
    const edited = message.editedAt && message.createdAt !== message.editedAt;
    const seen = !!message.seenAt;

    return (
        <Surface className={classNames} padding="none" variant="none">
            <div className="flex flex-col gap-1">
                <div className="flex gap-2 justify-between items-center">
                    <span className="text-(--secondary-fc) text-(length:--secondary-fs)">{isOwn ? "you" : "them"}</span>
                    <div className="flex gap-1 items-center">
                        {canEdit && <button className="p-0 bg-transparent" onClick={onEdit}><Edit className="text-(--secondary-fc) h-(--secondary-fs) w-(--secondary-fs)" /></button>}
                        {canDelete && <button className="p-0 bg-transparent" onClick={onDelete}><Trash2 className="text-(--secondary-fc) h-(--secondary-fs) w-(--secondary-fs)" /></button>}
                        {canReport && <button className="p-0 bg-transparent" onClick={onReport}><Flag className="text-(--secondary-fc) h-(--secondary-fs) w-(--secondary-fs)" /></button>}
                    </div>
                </div>
                {message.content && <MessageContent className="text-(--primary-fc) text-(length:--primary-fs) mx-auto" content={message.content} />}
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