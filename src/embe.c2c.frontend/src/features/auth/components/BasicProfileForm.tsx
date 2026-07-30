"use client";

import DateInput from "@/src/shared/components/inputs/date-input/DateInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import Surface from "@/src/shared/components/surfaces/Surface";
import { Gender, Location } from "@/src/shared/types/domain/value-objects";
import { Range } from "@/src/shared/types/range";
import * as enums from "@/src/shared/enums";
import LocationInput from "@/src/shared/components/inputs/location-input/LocationInput";
import TextAreaInput from "@/src/shared/components/inputs/text-area-input/TextAreaInput";

export type BasicProfileFormData = {
    birthDateRange: Range<string>;
    birthDate?: string;
    alias?: string;
    gender?: Gender;
    location?: Location;
}

export type BasicProfileFormConfig = {
    birthDate: boolean,
    alias: boolean,
    gender: boolean,
    location: boolean,
}

export type BasicProfileFormError = { [P in keyof BasicProfileFormData]?: string };
export type Mode = "register" | "edit";

export type BasicProfileFormProps = {
    error?: BasicProfileFormError;
    data: BasicProfileFormData;
    onChange: (data: BasicProfileFormData) => void;
    children?: React.ReactNode;
    className?: string;
    config?: BasicProfileFormConfig;
    mode?: Mode;
}

export default function BasicProfileForm({ 
    className, 
    data, 
    error, 
    onChange, 
    children, 
    config ,
    mode = "edit"
}: BasicProfileFormProps & { className?: string }) {

    config = config || {
        birthDate: true,
        alias: true,
        gender: false,
        location: false
    }

    const classNames = [
        "form",
        className
    ].filter(Boolean).join(" ");

    const genderOptions = enums.enumerate(Gender).map(gender => ({ value: gender.key, label: enums.formatGender(gender.value) }));
    const genderValue = enums.enumerate(Gender).find(gender => gender.value === data.gender)?.key;

    return (
        <Surface className={classNames} variant="inherit" padding="none">
            {
                config.alias &&
                <TextInput
                    label={"alias"}
                    value={data?.alias}
                    onBlur={(alias) => onChange({ ...data, alias })}
                    errorMessage={error?.alias}
                />
            }
            {
                config.birthDate &&
                <DateInput
                    errorMessage={error?.birthDate}
                    label={"date of birth"}
                    minDate={data.birthDateRange.lower}
                    maxDate={data.birthDateRange.upper}
                    value={data?.birthDate}
                    onBlur={(birthDate) => onChange({ ...data, birthDate })}
                />
            }
            {
                config.gender &&
                <SelectInput
                    required
                    info={mode === "edit" ? data.gender === undefined ? "specify your gender to receive more matches" : undefined : undefined}
                    infoType={mode === "edit" ? "important" : undefined}
                    errorMessage={error?.gender}
                    optionClassName="lowercase"
                    multiple={false}
                    options={genderOptions}
                    label={"gender"}
                    value={genderValue ? [genderValue] : []}
                    onChange={(gender) => onChange({ ...data, gender: gender.length > 0 ? enums.parse(Gender, gender[0]) : undefined })}
                />
            }
            {
                config.location &&
                <LocationInput
                    info={mode === "edit" ? data.location === undefined ? "specify your location to receive more matches" : undefined : undefined}
                    infoType={mode === "edit" ? "important" : undefined}
                    label="location"
                    errorMessage={error?.location}
                    value={data.location}
                    onChange={(location) => { onChange({ ...data, location }) }}
                />
            }
            {
                children
            }
        </Surface>
    )

}