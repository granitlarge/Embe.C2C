import Surface from "../../surfaces/Surface";

export type InputProps = {
    errorMessage?: string;
}

export type TextInputProps = InputProps & {
    className?: string;
    onChange?: (value: string) => void;
    value?: string;
    label?: React.ReactNode;
    type?: string;
    placeholder?: string;
    children?: React.ReactNode;
}

export default function TextInput({
    label,
    className,
    onChange,
    type = "text",
    value,
    errorMessage,
    placeholder,
    children
}: TextInputProps) {
    const shellClassNames = [
        className
    ].filter(Boolean).join(" ");
    return (
        <Surface className={`input-wrapper ${shellClassNames}`} padding="none" variant="inherit">
            {label && <span className="label">{label}</span>}
            <input
                className="input"
                placeholder={placeholder ?? ""}
                type={type}
                value={value ?? ""}
                onChange={(e) => onChange?.(e.target.value)}
            />
            {errorMessage && <span className="mx-auto text-(--error-fc)">{errorMessage}</span>}
            {children}
        </Surface>
    )
}