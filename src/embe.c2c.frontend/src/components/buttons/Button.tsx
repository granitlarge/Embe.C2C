export type ButtonProps = {
    children?: React.ReactNode;
    onClick?: () => void;
    className?: string;
}

export default function Button({ children, onClick, className }: ButtonProps) {
    const classNames = [
        "button",
        className
    ].filter(Boolean).join(" ");
    return (
        <button className={classNames} onClick={onClick}>
            {children}
        </button>
    );
}