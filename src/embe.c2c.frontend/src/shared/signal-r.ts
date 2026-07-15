import * as signalR from "@microsoft/signalr";
import { getAccessToken, refreshAccessToken } from "./security/functions";
import { ImageStatus } from "./types/domain/value-objects";
import { ReadDto } from "./types/dtos/types";
import { User, UserPermission } from "./types/domain/aggregates";
import useCurrentUserStore from "./stores/current-user";
import { Guid } from "./cache";
import { Images } from "lucide-react";

let connection: signalR.HubConnection | null = null;

export function getOrCreateConnectionOld() {

    if (!connection) {

        connection = new signalR.HubConnectionBuilder()
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

        connection.onclose(() => {
            connection = null;
        });

    }

    return connection;


}

export function getOrCreateConnection() {

    if (!connection) {

        connection = new signalR.HubConnectionBuilder()
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

        const tearDownHandlers = setupHandlers(connection);

        connection.onclose(() => {
            tearDownHandlers(connection!);
            connection = null;
        });

    } else {

    }

    return connection;

}

function setupHandlers
    (
        connection: signalR.HubConnection
    ): (connection: signalR.HubConnection) => void {

    const teardownImageHandler = setupImageHandlers(connection);

    return (connectionToTearDown) => {
        teardownImageHandler(connectionToTearDown);
    }

}

function setupImageHandlers(
    connection: signalR.HubConnection
): (connection: signalR.HubConnection) => void {

    const onImageStatusChanged = (imageId: Guid, newStatus: ImageStatus) => {

        const store = useCurrentUserStore.getState();
        const currentUser = store.currentUser;
        const setCurrentUser = store.setCurrentUser;

        if (!currentUser)
            return;

        if (newStatus === ImageStatus.Accepted) {

            setCurrentUser
                ({
                    ...currentUser,
                    data: {
                        ...currentUser.data,
                        images: currentUser.data.images?.map(image => {
                            if (image.id !== imageId) {
                                return image;
                            }
                            return {
                                ...image,
                                imageDetails: {
                                    ...image.imageDetails,
                                    status: newStatus
                                }
                            }
                        })
                    }
                });

        } else if (newStatus === ImageStatus.Rejected) {

            setCurrentUser({
                ...currentUser,
                data: {
                    ...currentUser.data,
                    images: currentUser.data.images?.filter(image => image.id !== imageId)
                }
            });

        }

    }

    connection.on("ImageStatusChanged", onImageStatusChanged);

    return (connectionToTearDown: signalR.HubConnection) => {
        connectionToTearDown.off("ImageStatusChanged", onImageStatusChanged);
    }

}