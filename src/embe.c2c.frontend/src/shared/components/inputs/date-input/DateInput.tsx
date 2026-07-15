import { useEffect, useState } from "react";
import Surface from "../../surfaces/Surface";
import ErrorMessage from "../ErrorMessage";

export type DateInputProps = {
    minDate?: string;
    maxDate?: string;
    label: string;
    value?: string;
    onBlur?: (value: string) => void;
    className?: string;
    required?: boolean
    errorMessage?: string;
    info?: string;
}

export default function DateInput({ label, value, onBlur, minDate, maxDate, className, required = true, errorMessage }: DateInputProps) {

    const [actualValue, setActualValue] = useState(value ?? "");

    const shellClassNames = [
        className
    ].filter(Boolean).join(" ");

    useEffect(() => {
        setActualValue(value ?? "")
    }, [value])

    const inputClassNames = [
        "input",
        (actualValue && ((minDate && actualValue < minDate) || (maxDate && actualValue > maxDate))) ? "input-invalid" : ""
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${shellClassNames}`} variant="inherit" padding="none">
            <span className="label">{label}</span>
            <input
                className={inputClassNames}
                type="date"
                value={actualValue}
                onChange={(e) => setActualValue(e.target.value)}
                onBlur={() => onBlur?.(actualValue)}
                min={minDate}
                max={maxDate}
                required={required} />
            <ErrorMessage message={errorMessage} />
        </Surface>
    )

}