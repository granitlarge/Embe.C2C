import DateInput from "@/src/shared/components/inputs/date-input/DateInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import { enumerate, parse } from "@/src/shared/enums";
import { Gender } from "@/src/shared/types/domain/value-objects";
import { Range } from "@/src/shared/types/range";

export type ProfileFormData = {
    birthDateRange: Range<string>;
    birthDate: string;
    gender: Gender;
}

export type ProfileFormProps = {
    data: ProfileFormData;
    onChange: (data: ProfileFormData) => void;
}

export default function ProfileForm({ data, onChange }: ProfileFormProps) {

    const year = new Date().getFullYear();
    const month = new Date().getMonth() + 1;
    const day = new Date().getDate();

    const minDate = `${year - 120}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;
    const maxDate = `${year - 18}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;

    const genders = enumerate(Gender).map(value => { return { value: value.key, label: value.key } });
    const gender = enumerate(Gender).find(value => value.value === data.gender)?.key || "";

    return (
        <div className="flex flex-col gap-3 items-center w-full">
            <DateInput
                label={"date of birth"}
                minDate={minDate}
                maxDate={maxDate}
                value={data.birthDate}
                onChange={(birthDate) => onChange({ ...data, birthDate })}
            />
            <SelectInput
                className="w-full" label="gender"
                options={genders}
                value={[gender]}
                onChange={(genders) => onChange({ ...data, gender: parse(Gender, genders[0]) })}
            />
        </div>
    )
}