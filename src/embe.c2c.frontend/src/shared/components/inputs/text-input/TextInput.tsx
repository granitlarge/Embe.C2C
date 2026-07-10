import { useState } from "react";
import Surface from "../../surfaces/Surface";

export type InputProps = {
    errorMessage?: string;
}

export type TextInputProps = InputProps & {
    className?: string;
    onBlur?: (value: string) => void;
    initialValue?: string;
    label?: React.ReactNode;
    type?: string;
    placeholder?: string;
    children?: React.ReactNode;
}
export default function TextInput({
    label,
    className,
    onBlur,
    type = "text",
    initialValue,
    errorMessage,
    placeholder,
    children
}: TextInputProps) {

    const [value, setValue] = useState(initialValue ?? "");

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
                value={value}
                onChange={(e) => { setValue(e.target.value); }}
                onBlur={(e) => { onBlur?.(value) }}
            />
            {errorMessage && <span className="mx-auto text-(length:--primary-fs) text-(--error-fc)">{errorMessage}</span>}
            {children}
        </Surface>
    )
}