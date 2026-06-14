import Surface from "../../surfaces/Surface";
import CheckboxInput from "../checkbox-input/CheckBoxInput";
import { InputProps } from "../text-input/TextInput";

export type Option = {
    value: string;
    label: string;
}

export type SelectInputProps = InputProps & {
    options: Option[];
    value?: string[];
    onChange?: (value: string[]) => void;
    label: string;
    className?: string;
    multiple?: boolean;
}

export default function SelectInput({ options, value, onChange, label, className, multiple = false, errorMessage }: SelectInputProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${classNames}`} variant="inherit" padding="none">
            <span className="label">{label}</span>
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
            {errorMessage && <span className="max-auto text-(--error-fc)">{errorMessage}</span>}
        </Surface>
    )
}