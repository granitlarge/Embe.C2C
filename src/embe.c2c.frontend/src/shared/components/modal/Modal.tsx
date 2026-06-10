import { useEffect, useRef } from "react";
import styles from "./Modal.module.css";

export type ModalProps = Omit<React.DetailedHTMLProps<React.DialogHTMLAttributes<HTMLDialogElement>, HTMLDialogElement>, "children" | "className" | "ref" | "closedby" | "hidden"> & {
    className?: string;
    children: React.ReactNode;
    closed: () => void;
    hidden: boolean;
    header: string;
};

export default function Modal({ children, className, closed, hidden, header, ...props }: ModalProps) {

    const classNames = [
        className,
        hidden ? "hidden" : ""
    ].filter(Boolean).join(" ");

    const dialog = useRef<HTMLDialogElement | null>(null);

    function close() {
        dialog.current?.close();
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
        <dialog closedby="any" ref={dialog} className={`
                ${classNames}
                flex flex-col items-center gap-5
                ${styles.modal}
                p-5 
                flex 
                flex-col 
                w-[70%] h-[70%] 
                m-auto 
                rounded-lg 
                bg-(--surface)
                scrollbar-gutter-stable
                `} {...props}>
            <h2 className="mr-auto ml-auto text-(--surface-font-color)">{header}</h2>
            {children}
        </dialog>
    );

}