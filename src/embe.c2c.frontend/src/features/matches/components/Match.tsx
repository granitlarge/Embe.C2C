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

function sortMessages(messages: ReadDto<MessageTypeDef, MessagePermission>[]): ReadDto<MessageTypeDef, MessagePermission>[] {
    return messages.sort((a, b) => new Date(a.data.createdAt ?? 0).getTime() - new Date(b.data.createdAt ?? 0).getTime());
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

    const [message, setMessage] = useState<{
        content: string;
        isEditing: boolean;
        editingId: Guid | undefined;
    }>({
        content: "",
        isEditing: false,
        editingId: undefined
    });

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
        const content = message.content;
        const isEditing = message.isEditing;
        const editingId = message.editingId;
        if (!content) {
            return;
        }

        if (isEditing) {

            if (!editingId) {
                throw new Error("impossible state");
            }

            const response = await updateMessage(editingId, content);
            if (response.success) {

                setMessages(prev => {
                    const otherMessages = prev.filter(message => message.data.id !== editingId);
                    return sortMessages([...otherMessages, response.value!]);
                });
                setMessage({
                    content: "",
                    isEditing: false,
                    editingId: undefined
                });

            } else {

                throw new Error("Not Implemented");

            }

        } else {

            const message: CreateMessage = {
                content: content,
                matchingId: match.data.id
            }

            const response = await createMessage(message);

            if (response.success) {
                setMessages(prev => sortMessages([...prev, response.value!]));
                setMessage({
                    content: "",
                    isEditing: false,
                    editingId: undefined
                });
            } else {
                throw new Error("Not Implemented");
            }

        }
    }

    function onReport(messageId: Guid) {
        throw new Error("Not Implemented");
    }

    function onEdit(message: MessageTypeDef) {
        setMessage({
            content: message.content!,
            isEditing: true,
            editingId: message.id
        });
    }

    async function onDelete(messageId: Guid) {
        const response = await deleteMessage(messageId);
        if (response.success) {
            if (message.isEditing && message.editingId === messageId) {
                setMessage({
                    content: "",
                    isEditing: false,
                    editingId: undefined
                });
            }
            setMessages(prev => prev.filter(message => message.data.id !== messageId));
        } else {
            throw new Error("Not Implemented");
        }
    }

    const items = messages.map(message => {
        const isOwn = message.data.authorUserId === user.userId;
        return (
            <li key={message.data.id}>
                <Message
                    className={isOwn ? "ml-auto" : "mr-auto"}
                    dto={message}
                    isOwn={isOwn}
                    onReport={() => onReport(message.data.id)}
                    onEdit={() => onEdit(message.data)}
                    onDelete={() => onDelete(message.data.id)}
                />
            </li>
        )
    }) ?? [];

    return (
        <div className={`flex flex-col justify-between gap-3 ${className}`}>
            <InfiniteScroll direction="up" className="flex flex-col gap-3 fs-group-primary" callback={loadMessages}>
                {items}
            </InfiniteScroll>
            <div className="relative flex gap-2">
                <TextAreaInput
                    value={message.content}
                    onChange={(value) => setMessage(prev => ({ ...prev, content: value }))}
                    placeholder="write a message.."
                    className="surface-secondary w-full p-3 rounded-lg grow-1"
                >
                </TextAreaInput>
                {
                    !message.isEditing &&
                    <button className="max-w-max max-h-max my-auto" onClick={saveMessage}>
                        <Send className="text-(--primary-fc) text-(length:--primary-fs)" />
                    </button>
                }
                {
                    message.isEditing &&
                    <div className="flex flex-col gap-2 justify-center">
                        <button className="" onClick={saveMessage}><Save /></button>
                        <button className="" onClick={() => setMessage({ content: "", isEditing: false, editingId: undefined })}><Ban /></button>
                    </div>
                }
            </div>
        </div>
    )
}