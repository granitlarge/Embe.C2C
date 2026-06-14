"use client";

import { InfiniteScroll } from "@/src/shared/components/infinite-scroll/InfiniteScroll";
import { Matching } from "@/src/shared/types/domain/aggregates"
import { AuthenticatedUser } from "@/src/shared/user";
import Message from "./Message";

export type MatchProps = {
    match: Matching,
    user: AuthenticatedUser
}

export default function Match({ match, user }: MatchProps) {

    match.conversation.messages?.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());

    async function loadMessages(): Promise<boolean> {
        return Promise.resolve(false);
    }

    const items = match.conversation.messages?.map(message => {
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