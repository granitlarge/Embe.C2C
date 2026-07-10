import { Check } from "@deemlol/next-icons";

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
        <div
            className={`input flex items-center justify-between w-full ${classNames}`}
            onClick={() => onChange?.(!value)}
        >
            <span className="w-(--primary-fs)" /> {/* left spacer */}

            <span className="flex-1 text-center">
                {label}
            </span>

            <Check className={`w-(--primary-fs) h-(--primary-fs) transition-opacity ${value ? "opacity-100" : "opacity-0"}`}
            />
        </div>
    )
}