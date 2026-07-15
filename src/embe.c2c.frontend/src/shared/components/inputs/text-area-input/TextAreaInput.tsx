import { useEffect, useState } from "react";
import styles from "./TextAreaInput.module.css";
import ErrorMessage from "../ErrorMessage";

export type TextAreaInputSize = "sm" | "md" | "lg";
export type TextAreaInputProps = Omit<React.PropsWithoutRef<React.TextareaHTMLAttributes<HTMLTextAreaElement>>, 'onBlur'> & {
    onBlur?: (value: string) => void;
    value?: string;
    label?: string;
    errorMessage?: string;
    className?: string;
    size?: TextAreaInputSize;
}

export default function TextAreaInput({ value, onBlur, label, className, errorMessage, size, ...props }: TextAreaInputProps) {
    const [actualValue, setActualValue] = useState(value ?? "");

    useEffect(() => {
        setActualValue(value ?? "");
    }, [value]);

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
                value={actualValue}
                onChange={(e) => {
                    setActualValue(e.target.value);
                }}
                onBlur={(e) => {
                    onBlur?.(actualValue);
                }}
                className={`input ${styles.textArea} ${textAreaClasses}`}
                {...props}
            >
            </textarea>
            <ErrorMessage message={errorMessage} />
        </div>
    )
}