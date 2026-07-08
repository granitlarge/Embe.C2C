import { useState } from "react";
import Surface from "../../surfaces/Surface";

export type DateInputProps = {
    minDate: string;
    maxDate: string;
    label: string;
    initialValue?: string;
    onBlur?: (value: string) => void;
    className?: string;
    required?: boolean
    errorMessage?: string;
}

export default function DateInput({ label, initialValue, onBlur, minDate, maxDate, className, required = true, errorMessage }: DateInputProps) {

    const [value, setValue] = useState(initialValue ?? "");

    const shellClassNames = [
        className
    ].filter(Boolean).join(" ");

    const inputClassNames = [
        "input w-full",
        (value && (value < minDate || value > maxDate)) ? "input-invalid" : ""
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${shellClassNames}`} variant="inherit" padding="none">
            <span className="label">{label}</span>
            <input
                className={inputClassNames}
                type="date"
                value={value}
                onChange={(e) => setValue(e.target.value)}
                onBlur={() => onBlur?.(value)}
                min={minDate}
                max={maxDate}
                required={required} />
            {errorMessage && <span className="mx-auto text-(--error-fc)">{errorMessage}</span>}
        </Surface>
    )

}