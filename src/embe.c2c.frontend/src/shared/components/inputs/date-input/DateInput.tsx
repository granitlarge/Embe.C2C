import Button from "../../buttons/Button";
import Surface from "../../surfaces/Surface";

export type DateInputProps = {
    minDate: string;
    maxDate: string;
    label: string;
    value?: string;
    onChange?: (value: string) => void;
    className?: string;
    required?: boolean
}

export default function DateInput({ label, value, onChange, minDate, maxDate, className, required = true }: DateInputProps) {

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
                onChange={(e) => onChange?.(e.target.value)}
                min={minDate}
                max={maxDate}
                required={required} />
        </Surface>
    )

}