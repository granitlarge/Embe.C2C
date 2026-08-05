import Button from "@/src/shared/components/buttons/Button";
import CheckboxInput from "@/src/shared/components/inputs/checkbox-input/CheckBoxInput";
import { useApplicationStore } from "@/src/shared/stores/provider";
import { useState } from "react";

import * as api from '../actions/action'

export type NotificationSettingsProps = {

}
export default function NotificationSettings({ }: NotificationSettingsProps) {

    const user = useApplicationStore(s => s.user);
    const setUser = useApplicationStore(s => s.setUser);

    const [email, setEmail] = useState(user?.data.settings?.emailNotifications!);
    const [device, setDevice] = useState(user?.data.settings?.deviceNotifications!);
    const [notifyOnLike, setNotifyOnLike] = useState(user?.data.settings?.notifyOnLike!);
    const [notifyOnMatch, setNotifyOnMatch] = useState(user?.data.settings?.notifyOnMatch!);
    const [notifyOnMessage, setNotifyOnMessage] = useState(user?.data.settings?.notifyOnMessage!);

    async function onSave() {

        const updateSettingsResponse = await api.updateSettings({
            deviceNotifications: device,
            emailNotifications: email,
            notifyOnLike: notifyOnLike,
            notifyOnMatch: notifyOnMatch,
            notifyOnMessage: notifyOnMessage
        });

        if (!updateSettingsResponse.success || !updateSettingsResponse.value) {
            console.log(updateSettingsResponse);
            throw new Error("not implemented");
        }

        setUser(_ => updateSettingsResponse.value);

    }

    return (
        <div className="flex flex-col gap-3">
            <CheckboxInput label="email notifications" value={email} onChange={setEmail} />
            <CheckboxInput label="device notifications" value={device} onChange={setDevice} />
            <CheckboxInput label="notify on new likes" value={notifyOnLike} onChange={setNotifyOnLike} />
            <CheckboxInput label="notify on new matches" value={notifyOnMatch} onChange={setNotifyOnMatch} />
            <CheckboxInput label="notify on new messages" value={notifyOnMessage} onChange={setNotifyOnMessage} />
            <Button intent="save" onClick={onSave}>save</Button>
        </div>
    )

}