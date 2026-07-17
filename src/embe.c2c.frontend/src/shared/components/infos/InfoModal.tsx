import { Info, AlertCircle } from "@deemlol/next-icons";
import { useState } from "react";
import Surface from "../surfaces/Surface";
import SmallModal from "../modal/SmallModal";
import Button from "../buttons/Button";

export type InfoType = "info" | "important";

export type InfoModalProps = {
    info: string;
    type: InfoType;
}
export default function InfoModal({ info, type }: InfoModalProps) {

    const [isOpen, setIsOpen] = useState(false);

    return (
        <div>
            <Button className="flex items-center max-w-max" onClick={() => setIsOpen(prev => !prev)}>
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