export type EmailInputProps = {
    className?: string;
    onChange?: (value: string) => void;
    value?: string;
    required?: boolean;
}

export function EmailInput({ className, onChange, value, required = false }: EmailInputProps) {
    return (
        <div className={`form flex flex-col gap-3 ${className} w-full`}>
            <label className="label flex flex-col items-center">
                <span className="label-text">email{required ? "*" : ""}</span>
                <input
                    className="input"
                    type="email"
                    placeholder="name@example.com"
                    value={value}
                    onChange={(e) => onChange?.(e.target.value)}
                    required={required ?? false} />
            </label>
        </div>
    )
}