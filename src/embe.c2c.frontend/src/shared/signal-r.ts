import * as signalR from "@microsoft/signalr";
import { getAccessToken, refreshAccessToken } from "./security/functions";

let connection: signalR.HubConnection | null = null;

export function getOrCreateConnection() {

    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(process.env.NEXT_PUBLIC_SIGNALR_HUB_URL!, {
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
    }

    connection.onclose(() => {
        connection = null;
    });

    return connection;

}