import { useEffect, useState } from "react";
import styles from "./TextAreaInput.module.css";

export type TextAreaInputSize = "sm" | "md" | "lg";
export type TextAreaInputProps = Omit<React.PropsWithoutRef<React.TextareaHTMLAttributes<HTMLTextAreaElement>>, 'onBlur'> & {
    onBlur?: (value: string) => void;
    initialValue?: string;
    label?: string;
    errorMessage?: string;
    className?: string;
    size?: TextAreaInputSize;
}

export default function TextAreaInput({ initialValue, onBlur, label, className, errorMessage, size, ...props }: TextAreaInputProps) {
    const [value, setValue] = useState(initialValue ?? "");

    useEffect(() => {
        setValue(initialValue ?? "");
    }, [initialValue]);

    const classNames = [
        className
    ].filter(Boolean).join(" ")

    const textAreaClasses = [
        !size ? "" : 
        size === "sm" ? "h-[50px]" :
        size === "md" ? "h-[100px]" :
        size === "lg" ? "h-[150px]" : ""
    ].filter(Boolean).join(" ")
    
    return (
        <div className={`input-wrapper ${classNames}`}>
            {label && <label className="label">{label}</label>}
            <textarea
                placeholder={props.placeholder}
                value={value}
                onChange={(e) => {
                    setValue(e.target.value);
                }}
                onBlur={(e) => {
                    onBlur?.(value);
                }}
                className={`input ${styles.textArea} ${textAreaClasses}`}
                {...props}
            >
            </textarea>
            {errorMessage && <span className="mx-auto text-(length:--primary-fs) text-(--error-fc)">{errorMessage}</span>}
        </div>
    )
}