"use client";

import BackButton from "@/src/shared/components/buttons/BackButton"
import Button from "@/src/shared/components/buttons/Button"
import CollapsibleSection from "@/src/shared/components/sections/CollapsibleSection"
import * as api from "@/src/features/me/actions/action"
import { useRouter } from "nextjs-toploader/app"
import ResetPasswordForm from "../../auth/components/ResetPasswordForm";
import ChangeEmailForm from "./ChangeEmailForm";
import AlertDialog from "@/src/shared/components/infos/AlertDialog";
import NotificationSettings from "./NotificationSettings";

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

    async function onResetPassword(newPassword: string) {
        const response = await api.resetPassword(newPassword);
        if (!response.success) {
            throw new Error("not implemented");
        }
    }

    return (

        <div className="flex flex-col gap-3">
            <div className="flex justify-between items-center">
                <BackButton />
                <h1>settings</h1>
            </div>
            <div className="flex flex-col gap-3">
                <CollapsibleSection headingLevel={2} title="account">
                    <CollapsibleSection headingLevel={3} title="reset password">
                        <ResetPasswordForm onReset={onResetPassword} />
                    </CollapsibleSection>
                    <CollapsibleSection headingLevel={3} title="change email">
                        <ChangeEmailForm />
                    </CollapsibleSection>
                </CollapsibleSection>
                <CollapsibleSection headingLevel={2} title="notifications">
                    <NotificationSettings />
                </CollapsibleSection>
                <Button intent="default" onClick={onLogout}>
                    logout
                </Button>
                <AlertDialog
                    title="are you sure?"
                    description="are you sure you'd like to delete your account?"
                    onConfirm={onDelete}
                    onCancel={() => { }}
                    confirmIntent="destructive"
                >
                    <Button intent="destructive">delete account</Button>
                </AlertDialog>
            </div>
        </div>

    )
}