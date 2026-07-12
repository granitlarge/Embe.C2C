import ErrorMessage from "../ErrorMessage";

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
    optionClassName?: string
    className?: string
}
export default function DropDownInput({ className, label, options, value, onChange, placeholder, errorMessage, optionClassName }: DropDownInputProps) {
    const classNames=[
        className
    ].filter(Boolean).join(" ");
    const optionClassNames = [
        optionClassName
    ].filter(Boolean).join(" ");
    return (
        <div className={`input-wrapper ${classNames}`}>
            {label && <label className="label">{label}</label>}
            <select value={value ?? ""} onChange={(e) => onChange?.(e.target.value)} className="input">
                <option className={optionClassNames} value={""} disabled>{placeholder}</option>
                {
                    options.map((option) => (
                        <option key={option.value} value={option.value} className={optionClassNames}>
                            {option.label}
                        </option>
                    ))
                }
            </select>
            <ErrorMessage message={errorMessage} />
        </div>
    )
}