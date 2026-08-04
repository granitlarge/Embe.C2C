export type ErrorMessageProps = {
    message?: string;
    className?: string;
}
export default function ErrorMessage({ message, className }: ErrorMessageProps) {
    const classNames = [className].filter(Boolean).join(" ");
    return (
        message &&
        <span className={`${classNames} text-center mx-auto text-(length:--secondary-fs) text-(--error-fc)`}>
            {message}
        </span>
    )
}