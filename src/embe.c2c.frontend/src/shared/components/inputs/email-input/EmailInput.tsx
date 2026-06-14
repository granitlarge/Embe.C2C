import Surface from "../../surfaces/Surface";

export type EmailInputProps = {
    className?: string;
    onChange?: (value: string) => void;
    value?: string;
    valid?: boolean;
    errorMessage?: string;
}

export function EmailInput({ 
    className, 
    onChange, 
    value, 
    valid = true,
    errorMessage,
}: EmailInputProps) {

    const shellClassNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${shellClassNames}`} variant="inherit" padding="none">
            <span className="label">email</span>
            <input
                className="input"
                type="email"
                placeholder="name@example.com"
                value={value ?? ""}
                onChange={(e) => onChange?.(e.target.value)}
            />
            {errorMessage && <span className="mx-auto text-(--error-fc)">{errorMessage}</span>}
        </Surface>
    )
}