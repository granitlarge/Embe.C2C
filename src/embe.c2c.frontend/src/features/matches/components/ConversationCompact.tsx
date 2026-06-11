import { Conversation } from "@/src/shared/types/domain/entities";
import MessageCompact from "./MessageBrief";
import Surface from "@/src/shared/components/surfaces/Surface";
import { AuthenticatedUser } from "@/src/shared/user";

export type ConversationCompactProps = {
    className?: string;
    conversation: Conversation;
    user: AuthenticatedUser;
}

export default function ConversationCompact({ className, conversation, user }: ConversationCompactProps) {
    const classNames = [className].filter(Boolean).join(" ");
    return (
        <Surface className={`${classNames} w-full flex flex-col justify-center`} padding="sm">
            <MessageCompact transparent={true} message={conversation.lastMessage} user={user} />
        </Surface>
    )
}