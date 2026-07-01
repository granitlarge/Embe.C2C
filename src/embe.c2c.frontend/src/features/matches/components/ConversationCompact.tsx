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

        <Surface className={`${classNames} w-full flex flex-col justify-center`} padding="none" variant="inherit">
            {
                conversation.lastMessage && <MessageCompact className="grow-1" messageDto={conversation.lastMessage} user={user} /> ||
                <span className="surface-tertiary text-(--primary-fc) text-center text-(length:--primary-fs) w-full grow-1 flex items-center justify-center rounded-md">
                    no messages yet
                </span>
            }
        </Surface>

    )

}