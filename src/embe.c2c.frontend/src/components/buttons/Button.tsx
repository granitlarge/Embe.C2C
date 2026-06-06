export type ButtonProps = {
    children?: React.ReactNode;
    onClick?: () => void;
    className?: string;
    disabled?: boolean;
}

export default function Button({ children, onClick, className, disabled = false}: ButtonProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ");
    return (
        <button disabled={disabled} className={`${classNames} button w-full`} onClick={onClick}>
            {children}
        </button>
    );
}