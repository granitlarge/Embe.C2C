"use client";

import { InfiniteScroll } from "@/src/shared/components/infinite-scroll/InfiniteScroll";
import { Matching } from "@/src/shared/types/domain/aggregates"
import { AuthenticatedUser } from "@/src/shared/user";
import Message from "./Message";
import { useState } from "react";
import { getMessages } from "../actions/action";
import { Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";

function sortMessages(messages: MessageTypeDef[]): MessageTypeDef[] {
    return messages.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
}

export type MatchProps = {
    match: Matching,
    user: AuthenticatedUser
}
export default function Match({ match, user }: MatchProps) {

    const [messages, setMessages] = useState(sortMessages(match.conversation.messages || []));
    const page = messages.length > 0 ? 2 : 1;
    const pageSize = messages.length > 0 ? messages.length : 50;

    async function loadMessages(): Promise<boolean> {
        const response = await getMessages(match.conversation.id, page, pageSize);
        if (response.success) {
            const newMessages = response.value || [];
            setMessages(prev => sortMessages([...newMessages, ...prev]));
            return newMessages.length == pageSize;
        } else {
            throw new Error("Not Implemented");
        }
    }

    const items = messages.map(message => {
        const isOwn = message.authorUserId === user.userId;
        return (
            <li key={message.id}>
                <Message className={isOwn ? "ml-auto" : "mr-auto"} message={message} isOwn={isOwn} />
            </li>
        )
    }) ?? [];

    return (
        <InfiniteScroll direction="up/left" className="flex flex-col gap-3 fs-group-primary" callback={loadMessages}>
            {items}
        </InfiniteScroll>
    )

}