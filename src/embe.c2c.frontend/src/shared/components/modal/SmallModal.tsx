import { useEffect, useRef } from "react";
import styles from "./SmallModal.module.css";

type ExcludeProps = "children" | "className" | "ref" | "closedby" | "hidden" | "onClick";
export type SmallModalProps = Omit<React.DetailedHTMLProps<React.DialogHTMLAttributes<HTMLDialogElement>, HTMLDialogElement>, ExcludeProps> & {
    className?: string;
    children: React.ReactNode;
    closed: () => void;
    hidden: boolean;
};

export default function SmallModal({ children, className, closed, hidden, ...props }: SmallModalProps) {



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

    const classNames = [
        className,
        hidden ? "hidden" : ""
    ].filter(Boolean).join(" ");
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
                if (e.target === e.currentTarget) {
                    dialog.current?.close();
                }
            }}
            {...props}
        >
            {children}
        </dialog>
    );

}