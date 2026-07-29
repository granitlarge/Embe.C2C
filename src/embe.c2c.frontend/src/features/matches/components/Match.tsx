"use client";

import { InfiniteScroll } from "@/src/shared/components/scroll/infinite-scroll/InfiniteScroll";
import { MessageCreatedNotification, MessagePermission, NotificationType, User } from "@/src/shared/types/domain/aggregates"
import { AuthenticatedUser } from "@/src/shared/user";
import Message from "./Message";
import { useEffect, useState } from "react";
import { createMessage, deleteMessage, getMessages, markMessageAsSeen, unmatch, updateMessage } from "../actions/action";
import { Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { CreateMessage, ReadDto } from "@/src/shared/types/dtos/types";
import { Guid } from "@/src/shared/cache";
import Surface from "@/src/shared/components/surfaces/Surface";
import { MessageCrafter } from "./MessageCrafter";
import * as DropdownMenu from "@radix-ui/react-dropdown-menu"
import { Ellipsis } from "lucide-react";
import Button from "@/src/shared/components/buttons/Button";
import Link from "@/src/shared/components/Links/Link";
import { useRouter } from "nextjs-toploader/app";
import { Routes } from "@/src/shared/routes";
import { useApplicationStore } from "@/src/shared/stores/provider";
import { markAsRead } from "@/src/shared/actions/notifications/action";
import BackButton from "@/src/shared/components/buttons/BackButton";

type MatchHeaderProps = {
    partner?: User,
    matchId: Guid
}
function MatchHeader({ partner, matchId }: MatchHeaderProps) {

    const router = useRouter();
    async function onUnmatch() {
        const response = await unmatch(matchId)
        if (!response) {
            throw new Error("not implemented");
        }
        router.refresh();
        router.push(Routes.protected.matches);
    }

    return (
        <header className="flex flex-row items-center justify-between">
            <BackButton />

            {
                partner &&
                <Link href={Routes.protected.user(partner.id)} className="no-underline text-(--primary-fc)">
                    <h1 className="truncate">{partner?.alias}</h1>
                </Link>
            }

            <DropdownMenu.Root modal={false}>
                <DropdownMenu.Trigger asChild>
                    <Button className="bg-transparent max-w-max p-0">
                        <Ellipsis />
                    </Button>
                </DropdownMenu.Trigger>

                <DropdownMenu.Portal>
                    <DropdownMenu.Content className="flex flex-col gap-1 surface-secondary p-2 rounded-md">
                        <DropdownMenu.Item>
                            <Button intent="destructive" onClick={onUnmatch}>
                                unmatch
                            </Button>
                        </DropdownMenu.Item>
                        <DropdownMenu.Item>
                            <Button intent="default">
                                block
                            </Button>
                        </DropdownMenu.Item>
                        <DropdownMenu.Item>
                            <Button intent="default">
                                report
                            </Button>
                        </DropdownMenu.Item>
                    </DropdownMenu.Content>
                </DropdownMenu.Portal>
            </DropdownMenu.Root>
        </header>
    )
}

export type MatchProps = {
    matchId: Guid
    user: AuthenticatedUser,
    className?: string;
}
export default function Match({ matchId, user, className }: MatchProps) {

    const router = useRouter();

    const matchings = useApplicationStore(s => s.matchings);
    const setMatchings = useApplicationStore(s => s.setMatchings);
    const notifications = useApplicationStore(s => s.notifications);
    const setNotifications = useApplicationStore(s => s.setNotifications);

    if (!matchings.find(m => m.data.id === matchId)!) {
        router.replace(Routes.protected.matches);
        throw new Error("Match not found in ApplicationStore.");
    }

    const match = matchings.find(m => m.data.id === matchId)!;
    const messages = match.data.messages ?? [];


    const partner = match.data.userId1 === user.userId ? match.data.user2?.data : match.data.user1?.data;

    const page = (messages.length) > 0 ? 2 : 1;
    const pageSize = (messages.length) > 0 ? messages.length : 50;

    const defaultMessageCrafterConfig = {
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
    }>(defaultMessageCrafterConfig);

    useEffect(() => {

        const unreadMessagesNotifications = notifications.filter(n =>
            n.data.type === NotificationType.MessageCreated &&
            match.data.messages?.some(mes => mes.data.id === (n.data as MessageCreatedNotification)?.messageId) &&
            n.data.isRead === false
        );

        markNotificationsAsRead(...unreadMessagesNotifications.map(umn => umn.data.id!));

    }, [notifications]);

    useEffect(() => {

        const unseenNewMessages = messages.filter(nm => !nm.data.seenAt && nm.data.authorUserId !== user.userId);
        markAsSeen(...unseenNewMessages.map(newMessage => newMessage.data.id));

        if (messageCrafterConfig.mode === "reply" && messages.every(m => m.data.id !== messageCrafterConfig.editingId)) {
            setMessageCrafterConfig(defaultMessageCrafterConfig);
        }

    }, [messages]);

    async function loadMessages(): Promise<boolean> {
        const response = await getMessages(match!.data.id, page, pageSize);
        if (!response.success) {
            throw new Error("not implemented");
        }

        const newMessages = response.value || [];
        setMatchings(prev => prev.map(m => {
            if (m.data.id !== match.data.id)
                return m;
            return {
                ...m,
                data: {
                    ...m.data,
                    messages: (m.data.messages ?? []).concat(newMessages)
                }
            }
        }))

        router.refresh();
        return newMessages.length == pageSize;
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
            if (!response.success)
                throw new Error("not immplemented");

            setMatchings(prev => prev.map(m => {
                if (m.data.id !== match.data.id)
                    return m;
                return {
                    ...m,
                    data: {
                        ...m.data,
                        messages: (m.data.messages ?? []).map(mes => {
                            if (mes.data.id !== response.value?.data.id)
                                return mes;
                            return response.value!;
                        })
                    }
                }
            }))

            setMessageCrafterConfig(defaultMessageCrafterConfig);

            router.refresh();

        } else {

            const message: CreateMessage = {
                content: content,
                matchingId: match!.data.id,
                replyToMessageId: replyId
            }

            const response = await createMessage(message);

            if (!response.success)
                throw new Error("not implemented");

            setMatchings(prev => prev.map(m => {
                if (m.data.id !== match.data.id)
                    return m;
                return {
                    ...m,
                    data: {
                        ...m.data,
                        messages: (m.data.messages ?? []).concat(response.value!)
                    }
                }
            }))

            setMessageCrafterConfig(defaultMessageCrafterConfig);
            router.refresh();

        }

    }

    async function markNotificationsAsRead(...notificationIds: Guid[]) {

        if (notificationIds.length === 0)
            return;

        const promises = notificationIds.map(n => markAsRead(n, true))
        const results = await Promise.all(promises);
        if (results.some(result => !result.success)) {
            throw new Error("not implemented");
        }

        setNotifications(prev => prev.map(n => {
            if (!notificationIds.includes(n.data.id!)) {
                return n;
            }
            return {
                ...n,
                data: {
                    ...n.data,
                    isRead: true,
                    readAt: new Date().toISOString()
                }
            }
        }))

    }

    async function markAsSeen(...messageIds: Guid[]) {

        if (messageIds.length === 0) {
            return;
        }

        const response = await markMessageAsSeen(...messageIds);
        if (!response.success)
            throw new Error("not implemented");

        setMatchings(prev => prev.map(m => {
            if (m.data.id !== match.data.id)
                return m;
            return {
                ...m,
                data: {
                    ...m.data,
                    messages: (m.data.messages ?? []).map(mes => {
                        if (!messageIds.includes(mes.data.id)) {
                            return mes;
                        }
                        return {
                            ...mes,
                            data: {
                                ...mes.data,
                                seenAt: new Date().toISOString()
                            }
                        }
                    })
                }
            }
        }));

        router.refresh();

    }

    function onReport(messageId: Guid) {
        throw new Error("Not Implemented");
    }

    function onEdit(message: MessageTypeDef) {

        if (messageCrafterConfig.mode === "edit") {
            setMessageCrafterConfig(defaultMessageCrafterConfig);
            return;
        }

        setMessageCrafterConfig({
            content: message.content!,
            mode: "edit",
            editingId: message.id,
            replyId: undefined
        });

    }

    function onReply(message: MessageTypeDef) {

        if (messageCrafterConfig.mode === "reply") {
            setMessageCrafterConfig(defaultMessageCrafterConfig);
            return;
        }

        setMessageCrafterConfig({
            content: "",
            editingId: undefined,
            replyId: message.id,
            mode: "reply"
        });

    }

    async function onDelete(messageId: Guid) {

        const response = await deleteMessage(messageId);
        if (!response.success) {
            throw new Error("not implemented");
        }

        if (messageCrafterConfig.mode === "edit" && messageCrafterConfig.editingId === messageId) {
            setMessageCrafterConfig(defaultMessageCrafterConfig);
        }

        const messagesThatReferenceDeletedMessage = messages.filter(m => m.data.replyToMessageId === messageId).map(m => m.data.id);
        setMatchings(prev => prev.map(m => {
            if (m.data.id !== match.data.id) {
                return m;
            }
            return {
                ...m,
                data: {
                    ...m.data,
                    messages: (m.data.messages ?? []).filter(mes => mes.data.id !== messageId).map(mes => {
                        if (messagesThatReferenceDeletedMessage.includes(mes.data.id!)) {
                            return {
                                ...mes,
                                data: {
                                    ...mes.data,
                                    replyToMessage: undefined,
                                    replyToMessageId: undefined
                                }
                            }
                        }
                        return mes;
                    })
                }
            }
        }))

        router.refresh();

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
            className={`${isOwn ? "ml-auto" : "mr-auto"} ${isOwn ? "surface-message" : "surface-secondary"}`}
            dto={message}
            isOwn={isOwn}
            onReport={() => onReport(message.data.id)}
            onEdit={() => onEdit(message.data)}
            onDelete={() => onDelete(message.data.id)}
            onReply={() => onReply(message.data)}
        />;

        if (isReply && !replyImmediatelyFollowsMessage) {
            item =
                <Surface className={`relative w-full px-2 py-1 flex flex-col gap-2`} padding="none" variant="tertiary">
                    <span className={`text-(length:--secondary-fs) text-(--secondary-fc) absolute ${isOwn ? "right-1" : "left-1"}`}>reply</span>
                    {

                        <>
                            {
                                isReplyDeleted ?
                                    <span className="text-(--secondary-fc) text-(length:--secondary-fs) italic mx-auto">replied-to message was deleted</span>
                                    :
                                    <Message className={`${isOwn ? "surface-secondary mr-auto" : "surface-message ml-auto"}`} dto={message.data.replyToMessage!} isOwn={!isOwn} />
                            }
                            <Message
                                className={`${isOwn ? "surface-message ml-auto" : "surface-secondary mr-auto"}`}
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
            <MatchHeader partner={partner} matchId={match.data.id} />
            <InfiniteScroll direction="up" className="flex flex-col gap-3 grow-1" callback={loadMessages}>
                {items}
            </InfiniteScroll>
            {
                /*
                partnerIsTyping && <span className="text-(--primary-fc) text-(length:--primary-fs) italic">{partner?.alias} is typing...</span>
                */
            }
            <MessageCrafter
                saveMessage={saveMessage}
                onCancel={() => setMessageCrafterConfig(defaultMessageCrafterConfig)}
                onChange={(value: string) => setMessageCrafterConfig(prev => ({ ...prev, content: value }))}
                content={messageCrafterConfig.content}
                mode={messageCrafterConfig.mode}
                replyToMessage={messageCrafterConfig.replyId ? messages.find(m => m.data.id === messageCrafterConfig.replyId) : undefined}
            />
        </div>
    )
}