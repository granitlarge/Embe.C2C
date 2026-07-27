"use client";

import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, ReactNode, RefObject, useEffect, useRef, useState } from "react"
import { getAccessToken, refreshAccessToken } from "../security/functions";
import { useApplicationStore } from "../stores/provider";
import { Guid } from "../cache";
import { ReadDto } from "../types/dtos/types";
import { Candidate, CandidatePermission, Matching, MatchingCreatedNotification, MatchingPermission, MessageCreatedNotification, NotificationType, User, UserPermission } from "../types/domain/aggregates";
import { getMatching, getMessage } from "@/src/features/matches/actions/action";
import { Notification } from "../types/domain/aggregates";
import { getNotification } from "../actions/notifications/action";
import { useRouter } from "nextjs-toploader/app";
import { AppRouterInstance } from "next/dist/shared/lib/app-router-context.shared-runtime";
import { getCandidate } from "../actions/candidates/action";

export interface SignalRProviderProps {
    children: ReactNode;
};

const SignalRContext = createContext<HubConnection | undefined>(undefined,);
export const SignalRProvider = ({
    children
}: SignalRProviderProps) => {

    const router = useRouter();

    const user = useApplicationStore(s => s.user);
    const setUser = useApplicationStore(s => s.setUser);

    const notifications = useApplicationStore(s => s.notifications);
    const setNotifications = useApplicationStore(s => s.setNotifications);

    const matchings = useApplicationStore(s => s.matchings);
    const setMatchings = useApplicationStore(s => s.setMatchings);

    const positiveJudgements = useApplicationStore(s => s.positiveJudgements);
    const setPositiveJudgements = useApplicationStore(s => s.setPositiveJudgements);

    const routerRef = useRef(router);

    const userRef = useRef(user);
    const setUserRef = useRef(setUser);

    const notificationsRef = useRef(notifications);
    const setNotificationsRef = useRef(setNotifications);

    const matchingsRef = useRef(matchings);
    const setMatchingsRef = useRef(setMatchings);

    const positiveJudgementsRef = useRef(positiveJudgements);
    const setPositiveJudgementsRef = useRef(setPositiveJudgements);

    const [connection, setConnection] = useState<HubConnection | undefined>(undefined);

    useEffect(() => {
        routerRef.current = router;
    }, [router]);

    useEffect(() => {
        userRef.current = user;
    }, [user])

    useEffect(() => {
        setUserRef.current = setUser;
    }, [setUser]);

    useEffect(() => {
        notificationsRef.current = notifications;
    }, [notifications])

    useEffect(() => {
        setNotificationsRef.current = setNotifications;
    }, [setNotifications])

    useEffect(() => {
        matchingsRef.current = matchings;
    }, [matchings])

    useEffect(() => {
        setMatchingsRef.current = setMatchings;
    }, [setMatchings])

    useEffect(() => {
        positiveJudgementsRef.current = positiveJudgements;
    }, [positiveJudgements])

    useEffect(() => {
        setPositiveJudgementsRef.current = setPositiveJudgements;
    }, [setPositiveJudgements]);

    useEffect(() => {
        const connection = createConnection();
        setConnection(connection);
        connection.start().catch(e => console.error(e));
        return () => {
            connection.stop().catch(e => console.error(e));
        };
    }, []);

    useEffect(() => {
        const removeUserHandlers = addUserHandlers(connection, routerRef, userRef, setUserRef);
        const removeMatchingHandlers = addMatchingHandlers(
            connection,
            routerRef,
            notificationsRef,
            setNotificationsRef,
            matchingsRef,
            setMatchingsRef
        );
        const removeNotificationHandlers = addNotificationHandlers(
            connection,
            routerRef,
            notificationsRef,
            setNotificationsRef,
        );
        const removePositiveJudgementsHandlers = addPositiveJudgementsHandlers(
            connection,
            routerRef,
            positiveJudgementsRef,
            setPositiveJudgementsRef
        );
        return () => {
            removeUserHandlers();
            removeMatchingHandlers();
            removeNotificationHandlers();
            removePositiveJudgementsHandlers();
        };
    }, [connection])

    return (
        <SignalRContext.Provider value={connection}>
            {children}
        </SignalRContext.Provider>
    )

}

function createConnection(): HubConnection {

    const connection = new HubConnectionBuilder()
        .withUrl(`${process.env.NEXT_PUBLIC_API_URL!}/hubs/main`, {
            accessTokenFactory: async () => {
                let accessToken = await getAccessToken();
                if (!accessToken) {
                    accessToken = (await refreshAccessToken())?.token;
                }
                if (!accessToken) {
                    throw new Error("Unable to obtain access token for SignalR connection.");
                }
                return accessToken;
            }
        })
        .withAutomaticReconnect()
        .build();
    return connection;

}

function addUserHandlers(
    connection: HubConnection | undefined,
    routerRef: RefObject<AppRouterInstance>,
    userRef: RefObject<ReadDto<User, UserPermission> | undefined>,
    setUserRef: RefObject<(newUser: ReadDto<User, UserPermission> | undefined) => void>
): () => void {

    return () => {

    }

}

function addMatchingHandlers(
    connection: HubConnection | undefined,
    routerRef: RefObject<AppRouterInstance>,
    notificationsRef: RefObject<ReadDto<Notification, NotificationPermission>[]>,
    setNotificationsRef: RefObject<(newNotifications: ReadDto<Notification, NotificationPermission>[]) => void>,
    matchingsRef: RefObject<ReadDto<Matching, MatchingPermission>[]>,
    setMatchingsRef: RefObject<(newMatchings: ReadDto<Matching, MatchingPermission>[]) => void>
) {

    const onMatchingCreated = async (matchingId: Guid) => {
        console.log("SignalR.MatchingCreated");
        const getMatchingResponse = await getMatching(matchingId);
        if (!getMatchingResponse.success)
            return;
        const matchings = matchingsRef.current;
        const setMatchings = setMatchingsRef.current;

        setMatchings(matchings.concat(getMatchingResponse.value!));
        routerRef.current.refresh();
    }

    const onMatchingRemoved = (matchingId: Guid) => {
        console.log("SignalR.MatchingRemoved");
        const notifications = notificationsRef.current;
        const setNotifications = setNotificationsRef.current;
        const matchings = matchingsRef.current;
        const setMatchings = setMatchingsRef.current;
        setMatchings(matchings.filter(m => m.data.id !== matchingId));
        setNotifications(notifications.filter(f =>
            f.data.type !== NotificationType.MatchingCreated ||
            (f.data as MatchingCreatedNotification).matchingId !== matchingId
        ));
    }

    const onMessageAddedHandler = async (messageId: Guid, matchingId: Guid) => {

        console.log("SignalR.MessageAdded");
        const matchings = matchingsRef.current;
        const setMatchings = setMatchingsRef.current;

        if (!matchings.some(m => m.data.id === matchingId))
            return;

        const getMessageResponse = await getMessage(messageId);
        if (!getMessageResponse.success || !getMessageResponse.value) {
            throw new Error("Not Implemented");
        }

        const newMessage = getMessageResponse.value;
        setMatchings(matchings.map(m => {
            if (m.data.id !== matchingId) {
                return m;
            }
            return {
                ...m,
                data: {
                    ...m.data,
                    lastMessage: newMessage,
                    messages: (m.data.messages ?? []).concat(newMessage),
                }
            }
        }))

        routerRef.current.refresh();

    };

    const onMessageEditedHandler = async (messageId: Guid, matchingId: Guid) => {

        console.log("SignalR.MessageEdited");
        const matchings = matchingsRef.current;
        const setMatchings = setMatchingsRef.current;

        if (!matchings.some(m => m.data.id === matchingId))
            return;

        const getMessageResponse = await getMessage(messageId);
        if (!getMessageResponse.success || !getMessageResponse.value) {
            throw new Error("Not Implemented");
        }

        const editedMessage = getMessageResponse.value;

        setMatchings(matchings.map(m => {
            if (m.data.id !== matchingId)
                return m;
            return {
                ...m,
                data: {
                    ...m.data,
                    messages: (m.data.messages ?? []).map(mes => {
                        if (mes.data.id !== messageId)
                            return mes;
                        return editedMessage;
                    })
                }
            }
        }))

        routerRef.current.refresh();

    };

    const onMessageDeletedHandler = (messageId: Guid, matchingId: Guid) => {
        console.log("SignalR.MessageDeleted");
        const matchings = matchingsRef.current;
        const setMatchings = setMatchingsRef.current;
        const notifications = notificationsRef.current;
        const setNotifications = setNotificationsRef.current;

        if (!matchings.some(m => m.data.id === matchingId))
            return;

        setMatchings(matchings.map(m => {
            if (m.data.id !== matchingId) {
                return m;
            }

            const messagesThatReferenceDeletedMessage = (m.data.messages ?? []).filter(m => m.data.replyToMessageId === messageId).map(m => m.data.id);

            return {
                ...m,
                data: {
                    ...m.data,
                    messages: (m.data.messages ?? []).filter(mes => mes.data.id !== messageId).map(mes => {
                        if (messagesThatReferenceDeletedMessage.includes(mes.data.id)) {
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
        }));

        setNotifications(notifications.filter(n => n.data.type !== NotificationType.MessageCreated || (n.data as MessageCreatedNotification)?.messageId !== messageId));

        routerRef.current.refresh();

    };

    const onMessagesSeenHandler = (messageIds: Guid[], matchingId: Guid) => {
        console.log("SignalR.MessageSeen");
        const matchings = matchingsRef.current;
        const setMatchings = setMatchingsRef.current;

        if (!matchings.some(m => m.data.id === matchingId))
            return;

        setMatchings(matchings.map(match => {
            if (match.data.id !== matchingId)
                return match;
            return {
                ...match,
                data: {
                    ...match.data,
                    messages: (match.data.messages ?? []).map(mes => {
                        if (!messageIds.includes(mes.data.id))
                            return mes;
                        return {
                            ...mes,
                            data: {
                                ...mes.data,
                                seenAt: new Date().toISOString()
                            }
                        };
                    })
                }
            }
        }))

        routerRef.current.refresh();

    };

    const onMessagesUnseenHandler = (messageIds: Guid[], matchingId: Guid) => {
        console.log("SignalR.MessageUnseen");
        const matchings = matchingsRef.current;
        const setMatchings = setMatchingsRef.current;

        if (!matchings.some(m => m.data.id === matchingId))
            return;

        setMatchings(matchings.map(match => {
            if (match.data.id !== matchingId)
                return match;
            return {
                ...match,
                data: {
                    ...match.data,
                    messages: (match.data.messages ?? []).map(mes => {
                        if (!messageIds.includes(mes.data.id))
                            return mes;
                        return {
                            ...mes,
                            data: {
                                ...mes.data,
                                seenAt: undefined
                            }
                        };
                    })
                }
            }
        }))

        routerRef.current.refresh();

    }

    connection?.on("MatchingCreated", onMatchingCreated);
    connection?.on("MatchingRemoved", onMatchingRemoved);

    connection?.on("MessageAdded", onMessageAddedHandler);
    connection?.on("MessageEdited", onMessageEditedHandler);
    connection?.on("MessageDeleted", onMessageDeletedHandler);
    connection?.on("MessagesSeen", onMessagesSeenHandler);
    connection?.on("MessagesUnseen", onMessagesUnseenHandler);

    return () => {

        connection?.off("MatchingCreated", onMatchingCreated);
        connection?.off("MatchingRemoved", onMatchingRemoved);

        connection?.off("MessageAdded", onMessageAddedHandler);
        connection?.off("MessageEdited", onMessageEditedHandler);
        connection?.off("MessageDeleted", onMessageDeletedHandler);
        connection?.off("MessagesSeen", onMessagesSeenHandler);
        connection?.off("MessagesUnseen", onMessagesUnseenHandler);
    }

}

function addNotificationHandlers(
    connection: HubConnection | undefined,
    routerRef: RefObject<AppRouterInstance>,
    notificationsRef: RefObject<ReadDto<Notification, NotificationPermission>[]>,
    setNotificationsRef: RefObject<(newNotifications: ReadDto<Notification, NotificationPermission>[]) => void>
): () => void {

    const onNotificationCreated = async (notificationId: Guid) => {

        console.log("SignalR.NotificationCreated");

        const notifications = notificationsRef.current;
        const setNotifications = setNotificationsRef.current;

        const getNotificationResponse = await getNotification(notificationId);
        if (!getNotificationResponse.success || !getNotificationResponse.value) {
            return;
        }

        setNotifications(notifications.concat(getNotificationResponse.value))
        routerRef.current.refresh();

    };

    const onNotificationRemoved = (notificationId: Guid) => {

        console.log("SignalR.NotificationRemoved");

        const notifications = notificationsRef.current;
        const setNotifications = setNotificationsRef.current;

        setNotifications(notifications.filter(n => n.data.id !== notificationId));
        routerRef.current.refresh();

    };

    connection?.on("NotificationCreated", onNotificationCreated);
    connection?.on("NotificationRemoved", onNotificationRemoved);

    return () => {
        connection?.off("NotificationCreated", onNotificationCreated);
        connection?.off("NotificationRemoved", onNotificationRemoved);
    };

}

function addPositiveJudgementsHandlers(connection: HubConnection | undefined,
    routerRef: RefObject<AppRouterInstance>,
    positiveJudgementsRef: RefObject<ReadDto<Candidate, CandidatePermission>[]>,
    setPositiveJudgementsRef: RefObject<(newCandidates: ReadDto<Candidate, CandidatePermission>[]) => void>
): () => void {

    const onPositivelyJudged = async (candidateId: Guid) => {

        console.log("SignalR.PositivelyJudged");
        const candidate = await getCandidate(candidateId);

        if (!candidate.success || !candidate.value) {
            return;
        }

        const positiveJudgements = positiveJudgementsRef.current;
        const setPositiveJudgements = setPositiveJudgementsRef.current;

        setPositiveJudgements(positiveJudgements.concat(candidate.value));
        routerRef.current.refresh();
    }

    connection?.on("PositivelyJudged", onPositivelyJudged);

    return () => {
        connection?.off("PositivelyJudged", onPositivelyJudged);
    }

}