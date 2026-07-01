import DualRangeInput from "@/src/shared/components/inputs/dual-range-input/DualRangeInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import { EngagementBoundedness, EngagementFrequency, EngagementMedium, Gender, RelationshipType } from "@/src/shared/types/domain/value-objects";
import { parse, enumerate } from "@/src/shared/enums";
import { Range } from "@/src/shared/types/range";
import SingleRangeInput from "@/src/shared/components/inputs/single-range-input/SingleRangeInput";
import Surface from "@/src/shared/components/surfaces/Surface";

export type SearchProfileBuilderFormData = {
    genders?: Gender[];
    ageRange?: Range<number>;
    distance?: number;
    engagementMedium?: EngagementMedium;
    engagementBoundedness?: EngagementBoundedness;
    engagementFrequency?: EngagementFrequency;
    possibleAgeRange: Range<number>;
    possibleDistanceRange: Range<number>;
}

export type SearchProfileBuilderFormError = { [P in keyof SearchProfileBuilderFormData]?: string };

export type SearchProfileBuilderFormProps = {
    data: SearchProfileBuilderFormData;
    error?: SearchProfileBuilderFormError;
    onChange: (data: SearchProfileBuilderFormData) => void;
    children: React.ReactNode;
    className?: string;
}
export default function SearchProfileBuilderForm({ className, children, data, error, onChange, }: SearchProfileBuilderFormProps) {

    const genders = enumerate(Gender).map(value => { return { value: value.key, label: value.key } });
    const selectedGenders = enumerate(Gender)
        .filter(gender => data.genders?.includes(gender.value) || false)
        .map(gender => gender.key);

    const possibleRelationshipTypes = enumerate(RelationshipType).map(value => { return { value: value.key, label: value.key } });
    const selectedRelationshipType = enumerate(RelationshipType)
        .filter(relationshipType => relationshipType.value === RelationshipType.Romantic)[0].key;

    const possibleEngagementMediums = enumerate(EngagementMedium).map(value => { return { value: value.key, label: value.key } });
    const selectedEngagementMedium = enumerate(EngagementMedium)
        .filter(engagementMedium => engagementMedium.value === EngagementMedium.Hybrid)[0].key;

    const possibleEngagementBoundedness = enumerate(EngagementBoundedness).map(value => { return { value: value.key, label: value.key } });
    const selectedEngagementBoundedness = enumerate(EngagementBoundedness)
        .filter(engagementBoundedness => engagementBoundedness.value === EngagementBoundedness.Ongoing)[0].key;

    const possibleEngagementFrequency = enumerate(EngagementFrequency).map(value => { return { value: value.key, label: value.key } });
    const selectedEngagementFrequency = enumerate(EngagementFrequency)
        .filter(engagementFrequency => engagementFrequency.value === EngagementFrequency.Weekly)[0].key;

    const classNames = [
        "form",
        className
    ].filter(Boolean).join(" ");

    return (

        <Surface className={classNames} variant="inherit" padding="none">
            <SelectInput
                multiple={false}
                options={possibleRelationshipTypes}
                value={[selectedRelationshipType]}
                label={"relationship type"}
            />
            <SelectInput
                multiple={false}
                options={possibleEngagementMediums}
                value={[selectedEngagementMedium]}
                label={"engagement medium"}
            />
            <SelectInput
                multiple={false}
                options={possibleEngagementBoundedness}
                value={[selectedEngagementBoundedness]}
                label={"engagement boundedness"}
            />
            <SelectInput
                multiple={false}
                options={possibleEngagementFrequency}
                value={[selectedEngagementFrequency]}
                label={"engagement frequency"}
            />
            <SelectInput
                options={genders}
                multiple={true}
                value={selectedGenders ?? []}
                onChange={(selected) => onChange({ ...data, genders: selected.map(value => parse(Gender, value)!) })} label={"genders"}
                errorMessage={error?.genders}
            />
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
            {children}
        </Surface>

    )

}