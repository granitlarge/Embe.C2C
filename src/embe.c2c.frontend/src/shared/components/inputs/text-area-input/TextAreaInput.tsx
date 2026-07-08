import { useEffect, useState } from "react";
import styles from "./TextAreaInput.module.css";

export type TextAreaInputProps = Omit<React.PropsWithoutRef<React.TextareaHTMLAttributes<HTMLTextAreaElement>>, 'onBlur'> & {
    onBlur?: (value: string) => void;
    initialValue?: string;
}

export default function TextAreaInput({ initialValue, onBlur,...props }: TextAreaInputProps) {
    const [value, setValue] = useState(initialValue ?? "");

    useEffect(() => {
        setValue(initialValue ?? "");
    }, [initialValue]);

    return (
        <textarea
            value={value}
            onChange={(e) => {
                setValue(e.target.value);
            }}
            onBlur={(e) => {
                onBlur?.(value);
            }}
            className={`input ${styles.textArea} ${props.className ?? ""}`}
            {...props}
        > 
        </textarea>
    )
}