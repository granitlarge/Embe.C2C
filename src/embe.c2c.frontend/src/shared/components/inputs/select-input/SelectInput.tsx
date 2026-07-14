import InfoModal, { InfoType } from "../../infos/InfoModal";
import Surface from "../../surfaces/Surface";
import CheckboxInput from "../checkbox-input/CheckBoxInput";
import ErrorMessage from "../ErrorMessage";
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
    required?: boolean
    optionClassName?: string;
    info?: string;
    infoType?: InfoType
}

export default function SelectInput({ 
    options, 
    value, 
    onChange, 
    label, 
    className, 
    multiple = false, 
    required = false, 
    errorMessage, 
    optionClassName ,
    info,
    infoType
}: SelectInputProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const optionClassNames = [
        optionClassName
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${classNames}`} variant="inherit" padding="none">
            <div className="flex items-center justify-center w-full gap-1">
                <span className="label m-0">{label}</span>
                {
                    info && infoType &&
                    <InfoModal info={info} type={infoType} />
                }
            </div>
            <div className="flex flex-col items-start gap-2 w-full">
                {
                    options.map((option) =>
                        <CheckboxInput
                            className={optionClassNames}
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
                                    if (checked) {
                                        onChange?.([option.value]);
                                    }
                                    else {
                                        onChange?.([]);
                                    }
                                }
                            }}
                        />)
                }
            </div>
            <ErrorMessage message={errorMessage} />
        </Surface>
    )
}