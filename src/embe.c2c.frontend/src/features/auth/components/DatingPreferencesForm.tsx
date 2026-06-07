import DualRangeInput from "@/src/shared/components/inputs/dual-range-input/DualRangeInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import { Gender } from "@/src/shared/types/domain/value-objects";
import { parse, enumerate } from "@/src/shared/enums";
import { Range } from "@/src/shared/types/range";

export type DatingPreferencesFormData = {
    genders: Gender[];
    ageRange: Range<number>;
    distanceRange: Range<number>;
    gendersError?: string;
}

export type DatingPreferencesFormProps = {
    data: DatingPreferencesFormData;
    onChange: (data: DatingPreferencesFormData) => void;
}

export default function DatingPreferencesForm({ data, onChange }: DatingPreferencesFormProps) {

    const genders = enumerate(Gender).map(value => { return { value: value.key, label: value.key } });
    const selectedGenders = enumerate(Gender)
        .filter(gender => data.genders.includes(gender.value))
        .map(gender => gender.key);

    const minAgeRange = 18;
    const maxAgeRange = 100;
    const minDistanceRange = 0;
    const maxDistanceRange = 160;

    return (
        <div className="form flex flex-col gap-3 w-full items-center">
            <SelectInput
                options={genders}
                multiple={true}
                value={selectedGenders}
                onChange={(selected) => onChange({ ...data, genders: selected.map(value => parse(Gender, value)!) })} label={"genders"}
                valid={data.gendersError === undefined}
                errorMessage={data.gendersError}
            />
            <div className="flex flex-col gap-8 w-full items-center">
                <DualRangeInput
                    label={"age"}
                    min={minAgeRange}
                    max={maxAgeRange}
                    step={1}
                    value={[data.ageRange.lower, data.ageRange.higher]}
                    minStepsBetweenThumbs={1}
                    onChange={(value: [number, number]) => onChange({ ...data, ageRange: { lower: value[0], higher: value[1] } })}
                />
                <DualRangeInput
                    label={"distance (km)"}
                    min={minDistanceRange}
                    max={maxDistanceRange}
                    step={1}
                    value={[data.distanceRange.lower, data.distanceRange.higher]}
                    minStepsBetweenThumbs={1}
                    onChange={(value: [number, number]) => onChange({ ...data, distanceRange: { lower: value[0], higher: value[1] } })}
                />
            </div>
        </div>
    )
}