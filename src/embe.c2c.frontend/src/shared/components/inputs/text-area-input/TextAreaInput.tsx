import styles from "./TextAreaInput.module.css";

export type TextAreaInputProps = Omit<React.PropsWithoutRef<React.TextareaHTMLAttributes<HTMLTextAreaElement>>, 'onChange'> & {
    onChange?: (value: string) => void;
    value?: string;
}

export default function TextAreaInput({ value, onChange, ...props }: TextAreaInputProps) {
    return (
        <textarea
            value={value ?? ""}
            onChange={(e) => onChange?.(e.target.value)}
            className={`input ${styles.textArea} ${props.className ?? ""}`}
            {...props}
        > 
        </textarea>
    )
}