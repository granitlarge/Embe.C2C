export type CheckboxInputProps = {
    value?: boolean;
    label: string;
    onChange?: (value: boolean) => void;
}

export default function CheckboxInput({ value = false, label, onChange }: CheckboxInputProps) {

    const classNames = [
        value ? "input-checked" : "",
    ].filter(Boolean).join(" ");
    return (
        <div className={`flex gap-2 input w-full ${classNames}`} onClick={() => {
            onChange?.(!value);
        }}>
            <span>{label}</span>
        </div>
    )
}