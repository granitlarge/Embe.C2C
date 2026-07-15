import * as signalR from "@microsoft/signalr";
import { getAccessToken, refreshAccessToken } from "./security/functions";
import { ImageStatus } from "./types/domain/value-objects";
import useCurrentUserStore from "./stores/current-user";
import { Guid } from "./cache";

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

        const targetImage = [...(currentUser.data.acceptedImages ?? []), ...(currentUser.data.pendingImages ?? [])].find(image => image.id === imageId);
        if (targetImage === undefined) {
            return;
        }

        if (newStatus === ImageStatus.Accepted) {

            setCurrentUser
                ({
                    ...currentUser,
                    data: {
                        ...currentUser.data,
                        acceptedImages: (currentUser.data.acceptedImages ?? []).concat([{
                            ...targetImage,
                            imageDetails: {
                                ...targetImage.imageDetails,
                                status: newStatus
                            }
                        }]),
                        pendingImages: (currentUser.data.pendingImages ?? []).filter(image => image.id !== imageId)
                    }
                });

        } else if (newStatus === ImageStatus.Rejected) {

            setCurrentUser({
                ...currentUser,
                data: {
                    ...currentUser.data,
                    pendingImages: (currentUser.data.pendingImages ?? []).filter(image => image.id !== imageId),
                    acceptedImages: (currentUser.data.acceptedImages ?? []).filter(image => image.id !== imageId)
                }
            });

        }

    }

    connection.on("ImageStatusChanged", onImageStatusChanged);

    return (connectionToTearDown: signalR.HubConnection) => {
        connectionToTearDown.off("ImageStatusChanged", onImageStatusChanged);
    }

}