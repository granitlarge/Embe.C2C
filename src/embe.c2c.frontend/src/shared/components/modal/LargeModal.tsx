import { useEffect, useRef } from "react";
import styles from "./LargeModal.module.css";

export type LargeModalProps = Omit<React.DetailedHTMLProps<React.DialogHTMLAttributes<HTMLDialogElement>, HTMLDialogElement>, "children" | "className" | "ref" | "closedby" | "hidden" | "onClick"> & {
    className?: string;
    children: React.ReactNode;
    closed: () => void;
    hidden: boolean;
    header?: string;
};

export default function LargeModal({ children, className, closed, hidden, header, ...props }: LargeModalProps) {

    const classNames = [
        className,
        hidden ? "hidden" : ""
    ].filter(Boolean).join(" ");

    const dialog = useRef<HTMLDialogElement | null>(null);

    function close() {
        closed();
    }

    useEffect(() => {
        if (hidden) {
            dialog.current?.close();
        } else {
            dialog.current?.showModal();
        }
        dialog.current?.addEventListener("close", close);
        return () => {
            dialog.current?.removeEventListener("close", close);
        }
    }, [closed, hidden]);

    return (
        <dialog ref={dialog} className={`
                ${classNames}
                flex flex-col
                ${styles.modal}
                m-auto 
                rounded-lg
                scrollbar-gutter-stable
                `}
            onClick={(e) => {
                e.stopPropagation();
                if (e.target === e.currentTarget) {
                    dialog.current?.close();
                }
            }}
            {...props}
        >
            {header && <h2 className="mx-auto text-(--primary-fc)">{header}</h2>}
            {children}
        </dialog>
    );

}