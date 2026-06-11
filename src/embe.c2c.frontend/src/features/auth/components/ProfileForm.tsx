import DateInput from "@/src/shared/components/inputs/date-input/DateInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
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
}

export default function ProfileForm({ data, error, onChange }: ProfileFormProps) {

    const genders = enumerate(Gender).map(value => { return { value: value.key, label: value.key } });
    const gender = enumerate(Gender).find(value => value.value === data?.gender)?.key || undefined;

    return (
        <div className="flex flex-col gap-3 items-center w-full">
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
        </div>
    )
}