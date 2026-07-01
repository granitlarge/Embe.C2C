import DateInput from "@/src/shared/components/inputs/date-input/DateInput";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import Surface from "@/src/shared/components/surfaces/Surface";
import { Range } from "@/src/shared/types/range";

export type BasicProfileFormData = {
    birthDateRange: Range<string>;
    birthDate?: string;
    alias?: string;
}

export type BasicProfileFormError = { [P in keyof BasicProfileFormData]?: string };

export type BasicProfileFormProps = {
    error?: BasicProfileFormError;
    data: BasicProfileFormData;
    onChange: (data: BasicProfileFormData) => void;
    children: React.ReactNode;
    className?: string;
}

export default function BasicProfileForm({ className, data, error, onChange, children }: BasicProfileFormProps & { className?: string }) {

    const classNames = [
        "form",
        className
    ].filter(Boolean).join(" ");

    return (
        <Surface className={classNames} variant="inherit" padding="none">
            <TextInput
                label={"alias"}
                value={data?.alias}
                onChange={(alias) => onChange({ ...data, alias })}
                errorMessage={error?.alias}
            />
            <DateInput
                label={"date of birth"}
                minDate={data.birthDateRange.lower}
                maxDate={data.birthDateRange.upper}
                value={data?.birthDate}
                onChange={(birthDate) => onChange({ ...data, birthDate })}
            />
            {
                children
            }
        </Surface>
    )
}