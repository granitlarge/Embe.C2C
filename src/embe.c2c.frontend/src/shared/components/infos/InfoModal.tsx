import { Info, AlertCircle } from "@deemlol/next-icons";
import { useState } from "react";
import Surface from "../surfaces/Surface";
import SmallModal from "../modal/SmallModal";

export type InfoType = "info" | "important";

export type InfoModalProps = {
    info: string;
    type: InfoType;
}
export default function InfoModal({ info, type }: InfoModalProps) {

    const [isOpen, setIsOpen] = useState(false);

    return (
        <div>
            <button className="flex items-center" onClick={() => setIsOpen(prev => !prev)}>
                {
                    type === "info" && <Info className="w-(--primary-fs) h-(--primary-fs)" /> ||
                    type === "important" && <AlertCircle className="w-(--primary-fs) h-(--primary-fs) text-(--important-fc)" />
                }
            </button>
            {
                isOpen &&
                <SmallModal
                    hidden={false}
                    closed={() => setIsOpen(false)}
                >
                    <Surface className="w-full h-full flex items-center justify-center">
                        <div className="flex justify-center items-center text-center">{info}</div>
                    </Surface>
                </SmallModal>
            }
        </div>
    )

}