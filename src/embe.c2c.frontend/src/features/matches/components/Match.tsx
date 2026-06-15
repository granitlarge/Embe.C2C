"use client";

import { InfiniteScroll } from "@/src/shared/components/infinite-scroll/InfiniteScroll";
import { Matching, MatchingPermission, MessagePermission } from "@/src/shared/types/domain/aggregates"
import { AuthenticatedUser } from "@/src/shared/user";
import Message from "./Message";
import { useState } from "react";
import { createMessage, getMessages } from "../actions/action";
import { Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { CreateMessage, ReadDto } from "@/src/shared/types/dtos/types";
import TextAreaInput from "@/src/shared/components/inputs/text-area-input/TextAreaInput";
import Button from "@/src/shared/components/buttons/Button";

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

    const [newMessage, setNewMessage] = useState("");

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

    async function sendMessage() {
        const message: CreateMessage = {
            content: newMessage,
            matchingId: match.data.id
        }
        const response = await createMessage(message);
        if (response.success) {
            setMessages(prev => sortMessages([...prev, response.value!]));
        } else {
            throw new Error("Not Implemented");
        }
    }

    const items = messages.map(message => {
        const isOwn = message.data.authorUserId === user.userId;
        return (
            <li key={message.data.id}>
                <Message className={isOwn ? "ml-auto" : "mr-auto"} dto={message} isOwn={isOwn} />
            </li>
        )
    }) ?? [];

    return (
        <div className={`flex flex-col justify-between gap-3 ${className}`}>
            <InfiniteScroll direction="up/left" className="flex flex-col gap-3 fs-group-primary" callback={loadMessages}>
                {items}
            </InfiniteScroll>
            <div className="relative">
                <TextAreaInput
                    value={newMessage}
                    onChange={setNewMessage}
                    placeholder="write a message.."
                    className="surface-secondary w-full p-2 rounded-lg"
                >
                </TextAreaInput>
                <Button className="absolute right-1 top-1/2 -translate-y-3/5 max-w-max" onClick={sendMessage}>send</Button>
            </div>
        </div>
    )
}