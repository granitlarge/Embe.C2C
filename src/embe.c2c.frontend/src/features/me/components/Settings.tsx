"use client";

import BackButton from "@/src/shared/components/buttons/BackButton"
import Button from "@/src/shared/components/buttons/Button"
import CollapsibleSection from "@/src/shared/components/sections/CollapsibleSection"
import * as api from "@/src/features/me/actions/action"
import { useRouter } from "nextjs-toploader/app"

export type SettingsProps = {

}
export default function Settings({ }: SettingsProps) {

    const router = useRouter();
    async function onLogout() {
        const response = await api.logout();
        if (!response.success) {
            throw new Error("not implemented");
        }
        router.push("/");
    }

    async function onDelete() {
        const response = await api.deleteAccount();
        if (!response.success) {
            throw new Error("not implemented");
        }
        router.push("/");
    }

    return (

        <div className="flex flex-col gap-3">
            <div className="flex justify-between items-center">
                <BackButton />
                <h1>settings</h1>
            </div>
            <div className="flex flex-col gap-3">
                <CollapsibleSection title="account">
                    account
                </CollapsibleSection>
                <CollapsibleSection title="notifications">
                    notifications
                </CollapsibleSection>
                <Button intent="default" onClick={onLogout}>
                    logout
                </Button>
                <Button intent="destructive" onClick={onDelete}>
                    delete account
                </Button>
            </div>
        </div>

    )
}