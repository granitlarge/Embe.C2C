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
}
export default function DropDownInput({ label, options, value, onChange, placeholder }: DropDownInputProps) {
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
        </div>
    )
}