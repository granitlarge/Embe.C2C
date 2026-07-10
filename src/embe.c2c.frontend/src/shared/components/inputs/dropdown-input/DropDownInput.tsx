export type Option = {
    label: string;
    value: string;
}

export type DropDownInputProps = {
    label?: string;
    placeholder: string;
    options: Option[];
    value?: string;
    onChange?: (value: string) => void;
    errorMessage?: string;
}
export default function DropDownInput({ label, options, value, onChange, placeholder, errorMessage }: DropDownInputProps) {
    return (
        <div className="input-wrapper">
            {label && <label className="label">{label}</label>}
            <select value={value ?? ""} onChange={(e) => onChange?.(e.target.value)} className="input">
                <option value={""} disabled>{placeholder}</option>
                {
                    options.map((option) => (
                        <option key={option.value} value={option.value}>
                            {option.label}
                        </option>
                    ))
                }
            </select>
            {errorMessage && <span className="mx-auto text-(length:--primary-fs) text-(--error-fc)">{errorMessage}</span>}
        </div>
    )
}