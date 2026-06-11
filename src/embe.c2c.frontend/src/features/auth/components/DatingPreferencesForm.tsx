import DualRangeInput from "@/src/shared/components/inputs/dual-range-input/DualRangeInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import { Gender } from "@/src/shared/types/domain/value-objects";
import { parse, enumerate } from "@/src/shared/enums";
import { Range } from "@/src/shared/types/range";
import SingleRangeInput from "@/src/shared/components/inputs/single-range-input/SingleRangeInput";

export type DatingPreferencesFormData = {
    genders?: Gender[];
    ageRange?: Range<number>;
    distance?: number;
    possibleAgeRange: Range<number>;
    possibleDistanceRange: Range<number>;
}

export type DatingPreferencesFormError = { [P in keyof DatingPreferencesFormData]?: string };

export type DatingPreferencesFormProps = {
    data: DatingPreferencesFormData;
    error?: DatingPreferencesFormError;
    onChange: (data: DatingPreferencesFormData) => void;
}

export default function DatingPreferencesForm({ data, error, onChange }: DatingPreferencesFormProps) {

    const genders = enumerate(Gender).map(value => { return { value: value.key, label: value.key } });
    const selectedGenders = enumerate(Gender)
        .filter(gender => data.genders?.includes(gender.value) || false)
        .map(gender => gender.key);

    return (
        <div className="form flex flex-col gap-3 w-full items-center">
            <SelectInput
                options={genders}
                multiple={true}
                value={selectedGenders ?? []}
                onChange={(selected) => onChange({ ...data, genders: selected.map(value => parse(Gender, value)!) })} label={"genders"}
                errorMessage={error?.genders}
            />
            <div className="flex flex-col gap-8 w-full items-center">
                <DualRangeInput
                    label={"age"}
                    min={data.possibleAgeRange.lower}
                    max={data.possibleAgeRange.upper}
                    step={1}
                    value={[data.ageRange?.lower ?? data.possibleAgeRange.lower, data.ageRange?.upper ?? data.possibleAgeRange.upper]}
                    minStepsBetweenThumbs={1}
                    onChange={(value: [number, number]) => onChange({ ...data, ageRange: { lower: value[0], upper: value[1] } })}
                />
                <SingleRangeInput
                    label={"distance (km)"}
                    min={data.possibleDistanceRange.lower}
                    max={data.possibleDistanceRange.upper}
                    step={1}
                    value={data.distance ?? data.possibleDistanceRange.upper}
                    onChange={(value: number) => onChange({ ...data, distance: value })}
                />
            </div>
        </div>
    )
}