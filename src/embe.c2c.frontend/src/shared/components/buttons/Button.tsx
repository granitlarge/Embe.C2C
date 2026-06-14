
export type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
}

export default function Button({...props}: ButtonProps) {
    const classNames = [
        "w-full",
        props.className
    ].filter(Boolean).join(" ");
    return (
        <button   {...props} className={classNames}>
            {props.children}
        </button>
    );
}