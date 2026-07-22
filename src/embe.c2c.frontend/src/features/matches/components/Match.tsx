"use client";

import { InfiniteScroll } from "@/src/shared/components/scroll/infinite-scroll/InfiniteScroll";
import { Matching, MatchingPermission, MessagePermission, User } from "@/src/shared/types/domain/aggregates"
import { AuthenticatedUser } from "@/src/shared/user";
import Message from "./Message";
import { useEffect, useRef, useState } from "react";
import { createMessage, deleteMessage, getMessage, getMessages, markMessageAsSeen, unmatch, updateMessage } from "../actions/action";
import { Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";
import { CreateMessage, ReadDto } from "@/src/shared/types/dtos/types";
import { Guid } from "@/src/shared/cache";
import Surface from "@/src/shared/components/surfaces/Surface";
import { getOrCreateConnectionOld } from "@/src/shared/signal-r";
import { HubConnection } from "@microsoft/signalr";
import { MessageCrafter } from "./MessageCrafter";
import * as DropdownMenu from "@radix-ui/react-dropdown-menu"
import { Ellipsis } from "lucide-react";
import Button from "@/src/shared/components/buttons/Button";
import Link from "@/src/shared/components/Links/Link";
import { useRouter } from "nextjs-toploader/app";

function sortMessages(messages: ReadDto<MessageTypeDef, MessagePermission>[]): ReadDto<MessageTypeDef, MessagePermission>[] {
    return messages.sort((a, b) => new Date(a.data.createdAt ?? 0).getTime() - new Date(b.data.createdAt ?? 0).getTime());
}

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
        router.push("/protected/matches");
    }

    return (
        <header className="flex flex-row items-center">
            {
                partner &&
                <Link href={`/protected/user/${partner.id}`} className="no-underline text-(--primary-fc)">
                    <h1 className="truncate">{partner?.alias}</h1>
                </Link>
            }

            <DropdownMenu.Root modal={false}>
                <DropdownMenu.Trigger asChild>
                    <Button className="bg-transparent max-w-max ml-auto p-0">
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
    match: ReadDto<Matching, MatchingPermission>,
    user: AuthenticatedUser,
    className?: string;
}
export default function Match({ match, user, className }: MatchProps) {

    const router = useRouter();
    const matchRef = useRef(match);
    const partner = match.data.userId1 === user.userId ? match.data.user2?.data : match.data.user1?.data;
    const connection = useRef<HubConnection | null>(null);

    const [partnerIsTyping, setPartnerIsTyping] = useState(false);
    const [messages, setMessages] = useState(sortMessages(match.data?.messages || []));
    const page = messages.length > 0 ? 2 : 1;
    const pageSize = messages.length > 0 ? messages.length : 50;

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

    function onMessageDeleted(messageId: Guid) {

        setMessages(prev =>
            sortMessages(
                prev.filter(dto => dto.data.id !== messageId)
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
            ));

        setMessageCrafterConfig(prev => {
            if (prev.mode === "reply" && prev.replyId === messageId) {
                return defaultMessageCrafterConfig;
            }
            return prev;
        });

        router.refresh();

    }

    function onMessagesSeen(...messageIds: Guid[]) {
        setMessages(prev =>
            sortMessages(
                prev.map(dto => {
                    if (messageIds.includes(dto.data.id)) {
                        return {
                            ...dto,
                            data: {
                                ...dto.data,
                                seenAt: new Date().toISOString()
                            }
                        }
                    } else {
                        return dto;
                    }
                })
            ));

        router.refresh();
    }

    function onMessagesUnseen(...messageIds: Guid[]) {
        setMessages(prev =>
            sortMessages(
                prev.map(dto => {
                    if (messageIds.includes(dto.data.id)) {
                        return {
                            ...dto,
                            data: {
                                ...dto.data,
                                seenAt: undefined
                            }
                        }
                    } else {
                        return dto;
                    }
                })
            ));
        router.refresh();
    }

    useEffect(() => {
        matchRef.current = match;
    }, [match]);

    useEffect(() => {

        connection.current = getOrCreateConnectionOld();

        const onMessageAddedHandler = async (messageId: Guid, matchingId: Guid) => {

            if (matchingId !== matchRef.current.data.id) {
                return;
            }

            const getMessageResponse = await getMessage(messageId);
            if (!getMessageResponse.success) {
                throw new Error("Not Implemented");
            }

            const newMessage = getMessageResponse.value!;
            setMessages(prev => {
                const otherMessages = prev.filter(m => m.data.id !== newMessage.data.id);
                return sortMessages([...otherMessages, newMessage]);
            });

            router.refresh();

        };

        const onMessageEditedHandler = async (messageId: Guid, matchingId: Guid) => {

            if (matchingId !== matchRef.current.data.id) {
                return;
            }

            const getMessageResponse = await getMessage(messageId);
            if (!getMessageResponse.success) {
                throw new Error("Not Implemented");
            }

            const editedMessage = getMessageResponse.value!;

            setMessages(prev => {
                const otherMessages = prev.filter(m => m.data.id !== editedMessage.data.id);
                return sortMessages([...otherMessages, editedMessage]);
            });

            router.refresh();

        };

        const onMessageDeletedHandler = (messageId: Guid, matchingId: Guid) => {

            if (matchingId !== matchRef.current.data.id) {
                return;
            }

            onMessageDeleted(messageId);

        };

        const onMessagesSeenHandler = (messageIds: Guid[], matchingId: Guid) => {

            if (matchingId !== matchRef.current.data.id) {
                return;
            }

            onMessagesSeen(...messageIds);

        };

        const onMessagesUnseenHandler = (messageIds: Guid[], matchingId: Guid) => {

            if (matchingId !== matchRef.current.data.id) {
                return;
            }

            onMessagesUnseen(...messageIds);

        }

        const onStartedTypingHandler = (matchingId: Guid) => {

            if (matchingId !== matchRef.current.data.id) {
                return;
            }

            setPartnerIsTyping(true);

        }

        const onStoppedTypingHandler = (matchingId: Guid) => {

            if (matchingId !== matchRef.current.data.id) {
                return;
            }

            setPartnerIsTyping(false);

        }

        connection.current.on("MessageAdded", onMessageAddedHandler);
        connection.current.on("MessageEdited", onMessageEditedHandler);
        connection.current.on("MessageDeleted", onMessageDeletedHandler);
        connection.current.on("MessagesSeen", onMessagesSeenHandler);
        connection.current.on("MessagesUnseen", onMessagesUnseenHandler);
        connection.current.on("StartedTyping", onStartedTypingHandler);
        connection.current.on("StoppedTyping", onStoppedTypingHandler);

        if (connection.current.state === "Disconnected") {
            connection.current.start().catch(err => {
                console.error("Failed to start connection:", err);
            });
        }

        return () => {
            connection.current?.off("MessageAdded", onMessageAddedHandler);
            connection.current?.off("MessageEdited", onMessageEditedHandler);
            connection.current?.off("MessageDeleted", onMessageDeletedHandler);
            connection.current?.off("MessagesSeen", onMessagesSeenHandler);
            connection.current?.off("MessagesUnseen", onMessagesUnseenHandler);
            connection.current?.off("StartedTyping", onStartedTypingHandler);
            connection.current?.off("StoppedTyping", onStoppedTypingHandler);
        }

    }, []);

    useEffect(() => {
        const unseenNewMessages = messages.filter(nm => !nm.data.seenAt && nm.data.authorUserId !== user.userId);
        markAsSeen(...unseenNewMessages.map(newMessage => newMessage.data.id));
    }, [messages]);

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
                setMessageCrafterConfig(defaultMessageCrafterConfig);

                router.refresh();

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
                setMessageCrafterConfig(defaultMessageCrafterConfig);
                router.refresh();
            } else {
                throw new Error("Not Implemented");
            }

        }

    }

    async function markAsSeen(...messageIds: Guid[]) {
        if (messageIds.length === 0) {
            return;
        }
        const response = await markMessageAsSeen(...messageIds);
        if (response.success) {
            onMessagesSeen(...messageIds);
        } else {
            throw new Error("Not Implemented");
        }
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
        if (response.success) {
            if (messageCrafterConfig.mode === "edit" && messageCrafterConfig.editingId === messageId) {
                setMessageCrafterConfig(defaultMessageCrafterConfig);
            }
            onMessageDeleted(messageId);
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
                                    <span className="text-(--secondary-fc) text-(length:--secondary-fs) italic mx-auto">replied message was deleted</span>
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
                partnerIsTyping && <span className="text-(--primary-fc) text-(length:--primary-fs) italic">{partner?.alias} is typing...</span>
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