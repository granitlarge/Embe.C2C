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
    errorMessage
}: EmailInputProps) {

    const shellClassNames = [
        className
    ].filter(Boolean).join(" ");
    const inputClassNames = [
        "input",
        !valid && "input-invalid"
    ].filter(Boolean).join(" ");

    return (
        <div className={`form flex flex-col gap-3 ${shellClassNames} w-full`}>
            <label className="label flex flex-col items-center">
                <span className="label-text">email</span>
                <input
                    className={inputClassNames}
                    type="email"
                    placeholder="name@example.com"
                    value={value ?? ""}
                    onChange={(e) => onChange?.(e.target.value)}
                    />
                {errorMessage && <span className="error-message">{errorMessage}</span>}
            </label>
        </div>
    )
}