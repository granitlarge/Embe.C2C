import styles from "./Modal.module.css";

export type ModalProps = Omit<React.DetailedHTMLProps<React.DialogHTMLAttributes<HTMLDialogElement>, HTMLDialogElement>, "children" | "className"> & {
    className?: string;
    children: React.ReactNode;
};

export default function Modal({ children, className, ...props }: ModalProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <dialog className={classNames} {...props}>
            {children}
        </dialog>
    );

}