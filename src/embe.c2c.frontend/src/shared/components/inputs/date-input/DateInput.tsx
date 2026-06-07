import Button from "../../buttons/Button";

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
        <div className={`flex flex-col gap-3 w-full ${shellClassNames}`}>
            <label className="label flex flex-col items-center w-full">
                <span className="label-text">{label}</span>
                <input
                    className={inputClassNames}
                    type="date"
                    value={value}
                    onChange={(e) => onChange?.(e.target.value)}
                    min={minDate}
                    max={maxDate}
                    required={required} />
            </label>
        </div>
    )

}