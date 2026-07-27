"use client";

import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, ReactNode, RefObject, useEffect, useRef, useState } from "react"
import { getAccessToken, refreshAccessToken } from "../security/functions";
import { useApplicationStore } from "../stores/provider";
import { Guid } from "../cache";
import { ReadDto } from "../types/dtos/types";
import { Matching, MatchingCreatedNotification, MatchingPermission, NotificationType, User, UserPermission } from "../types/domain/aggregates";
import { getMatching } from "@/src/features/matches/actions/action";
import { Notification } from "../types/domain/aggregates";
import { getNotification } from "../actions/notifications/action";

export interface SignalRProviderProps {
    children: ReactNode;
};

const SignalRContext = createContext<HubConnection | undefined>(undefined,);
export const SignalRProvider = ({
    children
}: SignalRProviderProps) => {


    const user = useApplicationStore(s => s.user);
    const setUser = useApplicationStore(s => s.setUser);
    const notifications = useApplicationStore(s => s.notifications);
    const setNotifications = useApplicationStore(s => s.setNotifications);
    const matchings = useApplicationStore(s => s.matchings);
    const setMatchings = useApplicationStore(s => s.setMatchings);

    const userRef = useRef(user);
    const setUserRef = useRef(setUser);
    const notificationsRef = useRef(notifications);
    const setNotificationsRef = useRef(setNotifications);
    const matchingsRef = useRef(matchings);
    const setMatchingsRef = useRef(setMatchings);

    const [connection, setConnection] = useState<HubConnection | undefined>(undefined);

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
        const connection = createConnection();
        setConnection(connection);
        connection.start().catch(e => console.error(e));
        return () => {
            connection.stop().catch(e => console.error(e));
        };
    }, []);

    useEffect(() => {
        const removeUserHandlers = addUserHandlers(connection, userRef, setUserRef);
        const removeMatchingHandlers = addMatchingHandlers(
            connection,
            notificationsRef,
            setNotificationsRef,
            matchingsRef,
            setMatchingsRef
        )
        const removeNotificationHandlers = addNotificationHandlers(
            connection,
            notificationsRef,
            setNotificationsRef,
        );
        return () => {
            removeUserHandlers();
            removeMatchingHandlers();
            removeNotificationHandlers();
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
    userRef: RefObject<ReadDto<User, UserPermission> | undefined>,
    setUserRef: RefObject<(newUser: ReadDto<User, UserPermission> | undefined) => void>
): () => void {

    return () => {

    }

}

function addMatchingHandlers(
    connection: HubConnection | undefined,
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

    connection?.on("MatchingCreated", onMatchingCreated);
    connection?.on("MatchingRemoved", onMatchingRemoved);

    return () => {

        connection?.off("MatchingCreated", onMatchingCreated);
        connection?.off("MatchingRemoved", onMatchingRemoved);

    }

}

function addNotificationHandlers(
    connection: HubConnection | undefined,
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

        setNotifications(notifications.concat(getNotificationResponse.value!))

    };

    connection?.on("NotificationCreated", onNotificationCreated);

    return () => {
        connection?.off("NotificationCreated", onNotificationCreated);
    };

}