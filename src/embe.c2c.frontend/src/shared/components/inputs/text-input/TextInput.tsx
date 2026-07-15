import { useEffect, useState } from "react";
import Surface from "../../surfaces/Surface";
import ErrorMessage from "../ErrorMessage";

export type InputProps = {
    errorMessage?: string;
}

export type TextInputProps = InputProps & {
    className?: string;
    onBlur?: (value: string) => void;
    value?: string;
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
    value,
    errorMessage,
    placeholder,
    children
}: TextInputProps) {

    const [actualValue, setActualValue] = useState(value ?? "");

    useEffect(() => {
        setActualValue(value || "");
    }, [value])
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
                value={actualValue}
                onChange={(e) => { setActualValue(e.target.value); }}
                onBlur={(e) => { onBlur?.(actualValue) }}
            />
            <ErrorMessage message={errorMessage} />
            {children}
        </Surface>
    )
}