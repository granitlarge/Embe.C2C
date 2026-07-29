import { useEffect, useState } from "react";
import Surface from "../../surfaces/Surface";
import ErrorMessage from "../ErrorMessage";
import InfoModal, { InfoType } from "../../infos/InfoModal";

export type InputProps = {
    errorMessage?: string;
}

export type TextInputProps = InputProps & {
    className?: string;
    onBlur?: (value: string) => void;
    value?: string;
    label?: React.ReactNode;
    type?: string;
    placeholder?: string;
    children?: React.ReactNode;
    info?: string;
    infoType?: InfoType
}
export default function TextInput({
    label,
    className,
    onBlur,
    type = "text",
    value,
    errorMessage,
    placeholder,
    children,
    info,
    infoType
}: TextInputProps) {

    const [actualValue, setActualValue] = useState(value ?? "");

    useEffect(() => {
        setActualValue(value || "");
    }, [value])
    const shellClassNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${shellClassNames}`} padding="none" variant="inherit">
            <div className="flex justify-center items-center gap-1">
                {label && <span className="label m-0">{label}</span>}
                {info && infoType && <InfoModal info={info} type={infoType} />}
            </div>
            <input
                className="input"
                placeholder={placeholder ?? ""}
                type={type}
                value={actualValue}
                onChange={(e) => { setActualValue(e.target.value); }}
                onBlur={(e) => { onBlur?.(actualValue) }}
            />
            <ErrorMessage message={errorMessage} />
            {children}
        </Surface>
    )
}