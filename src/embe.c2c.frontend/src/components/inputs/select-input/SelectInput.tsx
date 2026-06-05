import CheckboxInput from "../checkbox-input/CheckBoxInput";

export type Option = {
    value: string;
    label: string;
}

export type SelectInputProps = {
    options: Option[];
    value?: string[];
    onChange?: (value: string[]) => void;
    label: string;
    className?: string;
    multiple?: boolean;
}

export default function SelectInput({ options, value, onChange, label, className, multiple = false }: SelectInputProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <div className={`flex flex-col gap-3 w-full ${classNames}`}>
            <label className="label flex flex-col items-center gap-3 w-full">
                <span className="label-text">{label}</span>
                <div className="flex flex-col items-start gap-2 w-full">
                    {
                        options.map((option) =>
                            <CheckboxInput
                                key={option.value}
                                value={value?.includes(option.value)}
                                label={option.label}
                                onChange={(checked) => {
                                    if (multiple) {
                                        if (checked) {
                                            onChange?.([...(value || []), option.value]);
                                        } else {
                                            const newValues = (value || []).filter((v) => v !== option.value);
                                            onChange?.(newValues);
                                        }
                                    } else {
                                        onChange?.([option.value]);
                                    }
                                }}
                            />)
                    }
                </div>
            </label>
        </div>
    )
}