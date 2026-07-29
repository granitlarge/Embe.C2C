import * as RadixAlertDialog from "@radix-ui/react-alert-dialog"
import Button, { ButtonIntent } from "../buttons/Button";
export type AlertDialogProps = {
    children: React.ReactNode;
    title: string;
    description: string;
    onConfirm: () => void | Promise<void>;
    onCancel: () => void | Promise<void>;
    confirmIntent?: ButtonIntent
}
export default function AlertDialog({ confirmIntent, children, title, description, onConfirm, onCancel }: AlertDialogProps) {
    return (
        <RadixAlertDialog.Root>
            <RadixAlertDialog.Trigger asChild>
                {children}
            </RadixAlertDialog.Trigger>
            <RadixAlertDialog.Portal>
                <RadixAlertDialog.Overlay className="bg-white fixed inset-0" />
                <RadixAlertDialog.Content className="flex flex-col gap-3 surface-secondary fixed top-1/2 left-1/2 -translate-1/2 w-full p-3">
                    <div className="flex flex-col gap-0">
                        <RadixAlertDialog.Title className="mx-auto">
                            {title}
                        </RadixAlertDialog.Title>
                        <RadixAlertDialog.Description className="mx-auto  text-center text-(--secondary-fc) text-(length:--secondary-fs)">
                            {description}
                        </RadixAlertDialog.Description>
                    </div>
                    <div className="flex gap-2">
                        <RadixAlertDialog.Action asChild>
                            <Button intent={confirmIntent ?? "save"} onClick={onConfirm}>continue</Button>
                        </RadixAlertDialog.Action>
                        <RadixAlertDialog.Cancel asChild>
                            <Button intent="cancel" onClick={onCancel}>cancel</Button>
                        </RadixAlertDialog.Cancel>
                    </div>
                </RadixAlertDialog.Content>
            </RadixAlertDialog.Portal>
        </RadixAlertDialog.Root>
    )
}