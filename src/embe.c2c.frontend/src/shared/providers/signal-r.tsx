"use client";

import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { getAccessToken, refreshAccessToken } from "../security/functions";

export interface SignalRProviderProps {
    children: ReactNode;
};

const SignalRContext = createContext<HubConnection | undefined>(undefined,);
export const useSignalR = (): HubConnection | undefined => {
    const hubConnection = useContext(SignalRContext);
    return hubConnection;
}

export const SignalRProvider = ({
    children
}: SignalRProviderProps) => {

    const [connection, setConnection] = useState<HubConnection | undefined>(undefined);

    useEffect(() => {
        const connection = createConnection();
        setConnection(connection);
        connection.start().catch(e => console.error(e));
        return () => {
            connection.stop().catch(e => console.error(e));
        };
    }, []);

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
