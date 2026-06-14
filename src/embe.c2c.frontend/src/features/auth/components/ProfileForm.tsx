import Button from "@/src/shared/components/buttons/Button";
import DateInput from "@/src/shared/components/inputs/date-input/DateInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import Surface from "@/src/shared/components/surfaces/Surface";
import { enumerate, parse } from "@/src/shared/enums";
import { Gender } from "@/src/shared/types/domain/value-objects";
import { Range } from "@/src/shared/types/range";

export type ProfileFormData = {
    birthDateRange: Range<string>;
    birthDate?: string;
    gender?: Gender;
    userName?: string;
}

export type ProfileFormError = { [P in keyof ProfileFormData]?: string };

export type ProfileFormProps = {
    error?: ProfileFormError;
    data: ProfileFormData;
    onChange: (data: ProfileFormData) => void;
    children: React.ReactNode;
    className?: string;
}

export default function ProfileForm({ className, data, error, onChange, children}: ProfileFormProps & { className?: string }) {

    const classNames = [
        "form",
        className
    ].filter(Boolean).join(" ");
    const genders = enumerate(Gender).map(value => { return { value: value.key, label: value.key } });
    const gender = enumerate(Gender).find(value => value.value === data?.gender)?.key || undefined;

    return (
        <Surface className={classNames} variant="inherit" padding="none">
            <TextInput
                label={"username"}
                value={data?.userName}
                onChange={(userName) => onChange({ ...data, userName })}
                errorMessage={error?.userName}
            />
            <DateInput
                label={"date of birth"}
                minDate={data.birthDateRange.lower}
                maxDate={data.birthDateRange.upper}
                value={data?.birthDate}
                onChange={(birthDate) => onChange({ ...data, birthDate })}
            />
            <SelectInput
                label="gender"
                options={genders}
                value={gender ? [gender] : undefined}
                onChange={(genders) => onChange({ ...data, gender: parse(Gender, genders[0])! })}
            />
            {
                children
            }
        </Surface>
    )
}