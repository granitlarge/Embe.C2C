import { Info, AlertCircle } from "@deemlol/next-icons";
import { useState } from "react";
import Surface from "../surfaces/Surface";
import SmallModal from "../modal/SmallModal";
import Button from "../buttons/Button";

export type InfoType = "info" | "important";

export type InfoModalProps = {
    info: string;
    type: InfoType;
    className?: string;
}
export default function InfoModal({ className, info, type }: InfoModalProps) {

    const [isOpen, setIsOpen] = useState(false);

    const classNames = [className].filter(Boolean).join(" ");
    return (
        <div className={classNames}>
            <Button className="flex items-center max-w-max p-0" onClick={() => setIsOpen(prev => !prev)}>
                {
                    type === "info" && <Info className="w-(--primary-fs) h-(--primary-fs)" /> ||
                    type === "important" && <AlertCircle className="w-(--primary-fs) h-(--primary-fs) text-(--important-fc)" />
                }
            </Button>
            {
                isOpen &&
                <SmallModal
                    className="w-full"
                    hidden={false}
                    closed={() => setIsOpen(false)}
                >
                    <Surface className="w-full flex items-center justify-center" padding="lg">
                        <span className="text-center text-(length:--primary-fs) text-(--primary-fc)">{info}</span>
                    </Surface>
                </SmallModal>
            }
        </div>
    )

}