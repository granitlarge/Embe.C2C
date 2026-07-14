import { AlertTriangle } from "lucide-react";
import LargeModal from "../modal/LargeModal";
import { useState } from "react";

export type AlertProps = {
    children?: React.ReactNode;
}
export default function Alert({ children }: AlertProps) {

    const [open, setOpen] = useState(false);

    return (
        <span>
            <button className="max-w-max max-h-max flex justify-center items-center" onClick={() => setOpen(prev => !prev)}>
                <AlertTriangle className="w-(--primary-fs) h-(--primary-fs) text-(--primary-fc) text-(--warning-fc)" />
            </button>
            {
                open === true &&
                <LargeModal
                    closed={() => setOpen(false)}
                    hidden={false}
                >
                    {children}
                </LargeModal>
            }
        </span>
    )
}