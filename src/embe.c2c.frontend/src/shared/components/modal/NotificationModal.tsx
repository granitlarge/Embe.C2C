import { LocalStore } from "../../local-store";
import Button from "../buttons/Button";
import Surface from "../surfaces/Surface";
import LargeModal from "./LargeModal"
import { useEffect, useState } from "react";

export type NotificationModalProps = {
    closed: () => void;
    hidden: boolean;
}
export default function NotificationModal({ closed, hidden }: NotificationModalProps) {

    const [hiddenValue, setHiddenValue] = useState(hidden);

    useEffect(() => {
        setHiddenValue(hidden);
    }, [hidden])

    async function onEnableNotification() {
        const requestPermissionResult = await Notification.requestPermission();
        if (requestPermissionResult === "granted") {

        }        

        LocalStore.read().update((prev) => ({
            ...prev,
            askedForNotificationPermissions: true
        }));

        closed();
    }

    function skip() {
        LocalStore.read().update((prev) => ({
            ...prev,
            askedForNotificationPermissions: true
        }));
        setHiddenValue(true);
        closed();
    }

    return (
        <LargeModal closed={closed} hidden={hiddenValue}>
            <Surface className="flex flex-col gap-3 justify-center items-center h-full" variant="secondary">
                <span className="text-(--primary-fc) text-(length:--primary-fs) text-center">
                    enable notifications to receive updates on new matches, messages & likes
                </span>
                <Button onClick={onEnableNotification} intent="save">enable notifications</Button>
                <Button onClick={skip} intent="cancel">skip</Button>
            </Surface>
        </LargeModal>
    )
}