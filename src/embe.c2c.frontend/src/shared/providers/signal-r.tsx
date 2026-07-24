"use client";

import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, ReactNode, RefObject, useEffect, useRef, useState } from "react"
import { getAccessToken, refreshAccessToken } from "../security/functions";
import { useApplicationStore } from "../stores/provider";
import { Guid } from "../cache";
import { ImageStatus } from "../types/domain/value-objects";
import { ReadDto } from "../types/dtos/types";
import { User, UserPermission } from "../types/domain/aggregates";

export interface SignalRProviderProps {
    children: ReactNode;
};

const SignalRContext = createContext<HubConnection | undefined>(undefined,);
export const SignalRProvider = ({
    children
}: SignalRProviderProps) => {

    const user = useApplicationStore(s => s.user);
    const setUser = useApplicationStore(s => s.setUser);

    const userRef = useRef(user);
    const setUserRef = useRef(setUser);

    const [connection, setConnection] = useState<HubConnection | undefined>(undefined);

    useEffect(() => {
        userRef.current = user;
    }, [user])

    useEffect(() => {
        setUserRef.current = setUser;
    }, [setUser]);

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
        return () => {
            removeUserHandlers();
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