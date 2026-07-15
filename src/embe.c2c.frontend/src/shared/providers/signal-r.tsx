"use client";

import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, ReactNode, useEffect, useState } from "react"
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

    const [connection, setConnection] = useState<HubConnection | undefined>(undefined);

    useEffect(() => {
        const connection = createConnection();
        setConnection(connection);
        connection.start().catch(e => console.error(e));
        return () => {
            connection.stop().catch(e => console.error(e));
        };
    }, []);

    useEffect(() => {
        const removeUserHandlers = addUserHandlers(connection, user, setUser);
        return () => {
            removeUserHandlers();
        };
    }, [connection, user, setUser])

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
    user: ReadDto<User, UserPermission> | undefined,
    setUser: (newUser: ReadDto<User, UserPermission> | undefined) => void
): () => void {

    const onImageStatusChanged = (imageId: Guid, newStatus: ImageStatus) => {

        console.log("SignalR.ImageStatusChanged");
        if (!user) {
            return;
        }

        const targetImage = [...(user.data.acceptedImages ?? []), ...(user.data.pendingImages ?? [])].find(image => image.id === imageId);
        if (targetImage === undefined) {
            return;
        }

        if (newStatus === ImageStatus.Accepted) {

            setUser
                ({
                    ...user,
                    data: {
                        ...user.data,
                        acceptedImages: (user.data.acceptedImages ?? []).concat([{
                            ...targetImage,
                            imageDetails: {
                                ...targetImage.imageDetails,
                                status: newStatus
                            }
                        }]),
                        pendingImages: (user.data.pendingImages ?? []).filter(image => image.id !== imageId)
                    }
                });

        } else if (newStatus === ImageStatus.Rejected) {

            setUser({
                ...user,
                data: {
                    ...user.data,
                    pendingImages: (user.data.pendingImages ?? []).filter(image => image.id !== imageId),
                    acceptedImages: (user.data.acceptedImages ?? []).filter(image => image.id !== imageId)
                }
            });

        }

    }

    connection?.on("ImageStatusChanged", onImageStatusChanged);

    return () => {
        connection?.off("ImageStatusChanged", onImageStatusChanged);
    }

}