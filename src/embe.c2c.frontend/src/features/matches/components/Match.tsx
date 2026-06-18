"use client";

import { InfiniteScroll } from "@/src/shared/components/infinite-scroll/InfiniteScroll";
import { Matching, MatchingPermission, MessagePermission } from "@/src/shared/types/domain/aggregates"
import { AuthenticatedUser } from "@/src/shared/user";
import Message from "./Message";
import { useState } from "react";
import { createMessage, deleteMessage, getMessages, updateMessage } from "../actions/action";
import { Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { CreateMessage, ReadDto } from "@/src/shared/types/dtos/types";
import TextAreaInput from "@/src/shared/components/inputs/text-area-input/TextAreaInput";
import { Guid } from "@/src/shared/cache";
import { Save, Send } from "@deemlol/next-icons";
import { Ban } from "lucide-react";
import Surface from "@/src/shared/components/surfaces/Surface";

function sortMessages(messages: ReadDto<MessageTypeDef, MessagePermission>[]): ReadDto<MessageTypeDef, MessagePermission>[] {
    return messages.sort((a, b) => new Date(a.data.createdAt ?? 0).getTime() - new Date(b.data.createdAt ?? 0).getTime());
}

type MessageCrafterProps = {

    saveMessage: () => void;
    onChange: (value: string) => void;
    onCancel: () => void;
    content: string | undefined,
    editingId: Guid | undefined,
    replyId: Guid | undefined,
    replyToMessage: ReadDto<MessageTypeDef, MessagePermission> | undefined,
    mode: "create" | "edit" | "reply";

}

function MessageCrafter({
    saveMessage,
    content = undefined,
    replyToMessage = undefined,
    mode = "create",
    onChange,
    onCancel,
}: MessageCrafterProps) {

    return (
        <div className="relative flex gap-0">
            {
                mode === "reply" && <Message className="surface-tertiary absolute bottom-full mb-1" dto={replyToMessage!} isOwn={false} />
            }
            <TextAreaInput
                value={content}
                onChange={onChange}
                placeholder="write a message.."
                className="surface-secondary w-full p-3 rounded-l-lg grow-1"
            >
            </TextAreaInput>
            <div className="surface-secondary rounded-r-lg flex flex-col justify-center p-2">
                {
                    mode === "create" &&
                    <button className="max-w-max max-h-max my-auto" onClick={saveMessage}>
                        <Send className="text-(--primary-fc) text-(length:--primary-fs)" />
                    </button>
                }
                {
                    (mode === "edit" || mode === "reply") &&
                    <div className="flex flex-col gap-2 justify-center">
                        <button className="text-(--primary-fc) text-(length:--primary-fs)" onClick={saveMessage}>{mode === "edit" ? <Save /> : <Send />}</button>
                        <button className="text-(--primary-fc) text-(length:--primary-fs)" onClick={onCancel}><Ban /></button>
                    </div>
                }
            </div>
        </div>

    );

}

export type MatchProps = {
    match: ReadDto<Matching, MatchingPermission>,
    user: AuthenticatedUser,
    className?: string;
}
export default function Match({ match, user, className }: MatchProps) {

    const [messages, setMessages] = useState(sortMessages(match.data.conversation?.messages || []));
    const page = messages.length > 0 ? 2 : 1;
    const pageSize = messages.length > 0 ? messages.length : 50;

    const defaultMessage = {
        content: "",
        editingId: undefined,
        replyId: undefined,
        mode: "create" as "create" | "edit" | "reply",
    };

    const [messageCrafterConfig, setMessageCrafterConfig] = useState<{
        content: string;
        mode: "create" | "edit" | "reply";
        replyId: Guid | undefined;
        editingId: Guid | undefined;
    }>(defaultMessage);

    async function loadMessages(): Promise<boolean> {
        const response = await getMessages(match.data.id, page, pageSize);
        if (response.success) {
            const newMessages = response.value || [];
            setMessages(prev => sortMessages([...newMessages, ...prev]));
            return newMessages.length == pageSize;
        } else {
            throw new Error("Not Implemented");
        }
    }

    async function saveMessage() {
        const content = messageCrafterConfig.content;
        const editingId = messageCrafterConfig.editingId;
        const replyId = messageCrafterConfig.replyId;

        if (!content) {
            return;
        }

        if (messageCrafterConfig.mode === "edit") {

            if (!editingId) {
                throw new Error("impossible state");
            }

            const response = await updateMessage(editingId, content);
            if (response.success) {

                setMessages(prev => {
                    const otherMessages = prev.filter(message => message.data.id !== editingId);
                    return sortMessages([...otherMessages, response.value!]);
                });
                setMessageCrafterConfig(defaultMessage);

            } else {

                throw new Error("Not Implemented");

            }

        } else {

            const message: CreateMessage = {
                content: content,
                matchingId: match.data.id,
                replyToMessageId: replyId
            }

            const response = await createMessage(message);

            if (response.success) {
                setMessages(prev => sortMessages([...prev, response.value!]));
                setMessageCrafterConfig(defaultMessage);
            } else {
                throw new Error("Not Implemented");
            }

        }
    }

    function onReport(messageId: Guid) {
        throw new Error("Not Implemented");
    }

    function onEdit(message: MessageTypeDef) {
        setMessageCrafterConfig({
            content: message.content!,
            mode: "edit",
            editingId: message.id,
            replyId: undefined
        });
    }

    function onReply(message: MessageTypeDef) {
        setMessageCrafterConfig({
            content: "",
            editingId: undefined,
            replyId: message.id,
            mode: "reply"
        });
    }

    async function onDelete(messageId: Guid) {
        const response = await deleteMessage(messageId);
        if (response.success) {
            if (messageCrafterConfig.mode === "edit" && messageCrafterConfig.editingId === messageId) {
                setMessageCrafterConfig(defaultMessage);
            }
            setMessages(prev =>
                prev
                    .filter(dto => dto.data.id !== messageId)
                    .map(dto => {
                        if (dto.data.replyToMessageId === messageId) {
                            return {
                                ...dto,
                                data: {
                                    ...dto.data,
                                    replyToMessageId: undefined,
                                    replyToMessage: undefined
                                }
                            }
                        } else {
                            return dto;
                        }
                    })
            );
        } else {
            throw new Error("Not Implemented");
        }
    }

    const items = messages.map(message => {

        // This is way too complex. Simplify.
        const isOwn = message.data.authorUserId === user.userId;
        const isReply = message.data.isReply;
        const isReplyDeleted = !message.data.replyToMessageId;
        const indexOfMessage = messages.findIndex(m => m.data.id === message.data.id);
        const indexOfReply = messages.findIndex(m => m.data.id === message.data.replyToMessageId);
        const replyImmediatelyFollowsMessage = indexOfReply === indexOfMessage - 1;

        let item = <Message
            className={`max-w-max ${isOwn ? "ml-auto" : "mr-auto"} ${isOwn ? "surface-message" : "surface-secondary"}`}
            dto={message}
            isOwn={isOwn}
            onReport={() => onReport(message.data.id)}
            onEdit={() => onEdit(message.data)}
            onDelete={() => onDelete(message.data.id)}
            onReply={() => onReply(message.data)}
        />;

        if (isReply && !replyImmediatelyFollowsMessage) {
            item = <Surface className={`relative w-full px-2 py-1 flex flex-col gap-2`} padding="none" variant="tertiary">
                <span className={`text-(length:--secondary-fs) text-(--secondary-fc) absolute ${isOwn ? "right-1" : "left-1"}`}>reply</span>
                {

                    <>
                        {
                            isReplyDeleted ?
                                <span className="text-(--secondary-fc) text-(length:--secondary-fs) italic mx-auto">replied message was deleted</span>
                                :
                                <Message className={`${isOwn ? "surface-secondary mr-auto" : "surface-message ml-auto"} max-w-max`} dto={message.data.replyToMessage!} isOwn={!isOwn} />
                        }
                        <Message
                            className={`${isOwn ? "surface-message ml-auto" : "surface-secondary mr-auto"} max-w-max`}
                            dto={message}
                            isOwn={isOwn}
                            onReport={() =>
                                onReport(message.data.id)}
                            onEdit={() => onEdit(message.data)}
                            onDelete={() => onDelete(message.data.id)}
                            onReply={() => onReply(message.data)} />
                    </>
                }
            </Surface>
        }

        return (
            <li key={message.data.id}>
                {item}
            </li>
        )
    }) ?? [];

    return (
        <div className={`flex flex-col justify-between gap-3 ${className}`}>
            <InfiniteScroll direction="up" className="flex flex-col gap-3 fs-group-primary" callback={loadMessages}>
                {items}
            </InfiniteScroll>
            <MessageCrafter
                saveMessage={saveMessage}
                onCancel={() => setMessageCrafterConfig(defaultMessage)}
                onChange={(value: string) => setMessageCrafterConfig(prev => ({ ...prev, content: value }))}
                content={messageCrafterConfig.content}
                editingId={messageCrafterConfig.editingId}
                mode={messageCrafterConfig.mode}
                replyId={messageCrafterConfig.replyId}
                replyToMessage={messageCrafterConfig.replyId ? messages.find(m => m.data.id === messageCrafterConfig.replyId) : undefined}
            />
        </div>
    )
}