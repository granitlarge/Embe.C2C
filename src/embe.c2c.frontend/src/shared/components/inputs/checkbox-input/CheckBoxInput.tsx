export type CheckboxInputProps = {
    value?: boolean;
    label: string;
    onChange?: (value: boolean) => void;
}

export default function CheckboxInput({ value = false, label, onChange }: CheckboxInputProps) {

    const classNames = [
        value ? "input-checked" : ""
    ].filter(Boolean).join(" ");
    return (
        <div className={`input flex gap-2 w-full ${classNames}`} onClick={() => {
            onChange?.(!value);
        }}>
            <span className="mx-auto">{label}</span>
        </div>
    )
}