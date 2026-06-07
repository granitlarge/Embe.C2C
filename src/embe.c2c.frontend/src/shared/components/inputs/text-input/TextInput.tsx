export type TextInputProps = {
    className?: string;
    onChange?: (value: string) => void;
    value?: string;
    valid?: boolean;
    errorMessage?: string;
    label: React.ReactNode;
    type?: string;
    placeholder?: string;
}
export default function TextInput({
    label,
    className,
    onChange,
    type = "text",
    value,
    valid,
    errorMessage,
    placeholder
}: TextInputProps) {
    const shellClassNames = [
        className
    ].filter(Boolean).join(" ");
    const inputClassNames = [
        "input",
        !valid && "input-invalid"
    ].filter(Boolean).join(" ");
    return (
        <label className={`label flex flex-col items-center ${shellClassNames} w-full`}>
            <span className="label-text">{label}</span>
            <input
                className={inputClassNames}
                placeholder={placeholder ?? ""}
                type={type}
                value={value ?? ""}
                onChange={(e) => onChange?.(e.target.value)}
            />
            {!valid && errorMessage && <span className="error-message">{errorMessage}</span>}
        </label>
    )
}