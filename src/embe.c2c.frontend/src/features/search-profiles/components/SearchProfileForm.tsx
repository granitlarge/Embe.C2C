"use client";

import Button from "@/src/shared/components/buttons/Button";
import InfoSurface from "@/src/shared/components/infos/InfoSurface";
import DateInput from "@/src/shared/components/inputs/date-input/DateInput";
import DropDownInput from "@/src/shared/components/inputs/dropdown-input/DropDownInput";
import DualRangeInput from "@/src/shared/components/inputs/dual-range-input/DualRangeInput";
import SelectInput from "@/src/shared/components/inputs/select-input/SelectInput";
import SingleRangeInput from "@/src/shared/components/inputs/single-range-input/SingleRangeInput";
import TextAreaInput from "@/src/shared/components/inputs/text-area-input/TextAreaInput";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import Surface from "@/src/shared/components/surfaces/Surface"
import * as enums from "@/src/shared/enums";
import { EngagementBoundedness, EngagementFrequency, EngagementMedium, Gender, RelationshipType } from "@/src/shared/types/domain/value-objects";
import { useState } from "react";
import * as z from "zod";
import { createSearchProfile as createSearchProfile, deleteSearchProfile, updateSearchProfile } from "../actions";
import { SearchProfile, User } from "@/src/shared/types/domain/aggregates";
import CheckboxInput from "@/src/shared/components/inputs/checkbox-input/CheckBoxInput";
import { useRouter } from "nextjs-toploader/app";
import LocationInput from "@/src/shared/components/inputs/location-input/LocationInput";
import { Location } from "@/src/shared/types/domain/value-objects"
import { updateProfile } from "../../me/actions/action";
import { Routes } from "@/src/shared/routes";
import AlertDialog from "@/src/shared/components/infos/AlertDialog";

export type SearchProfileFormProps = {
    className?: string;
    searchProfile?: SearchProfile;
    user: User;
}
export default function SearchProfileForm({ className, searchProfile, user: initialUser }: SearchProfileFormProps) {

    const maxDistanceKm = 300;
    const minDistance = 1;
    const maxAge = 120;
    const minAge = 18;

    const router = useRouter();

    const initialState = {
        id: searchProfile?.id,
        name: searchProfile?.name || "",
        description: searchProfile?.description || "",
        relationship: searchProfile?.relationshipType as RelationshipType | undefined,
        medium: searchProfile?.engagement?.medium as EngagementMedium | undefined,
        duration: searchProfile?.engagement?.boundedness as EngagementBoundedness | undefined,
        frequency: searchProfile?.engagement?.frequency as EngagementFrequency | undefined,
        dateRange:
            (
                searchProfile?.engagement?.startDate && searchProfile?.engagement?.endDate ?
                    { startDate: searchProfile?.engagement?.startDate, endDate: searchProfile?.engagement?.endDate } : undefined
            ) as { startDate?: string, endDate?: string } | undefined,

        ageRange: (searchProfile?.ageRangeMin ? [searchProfile.ageRangeMin, searchProfile.ageRangeMax ?? maxAge] : [minAge, maxAge]) as [number, number | undefined],
        maximumDistanceKm: (searchProfile?.maximumDistanceKm ?? maxDistanceKm) as number | undefined,
        genders: searchProfile?.genders ?? [] as Gender[],
        active: searchProfile?.active ?? true,
        createdAt: searchProfile?.createdAt,
        updatedAt: searchProfile?.updatedAt
    }

    const [user, setUser] = useState(initialUser);

    const [serverSideState, setServerSideState] = useState(initialState);
    const [clientSideState, setClientSideState] = useState(initialState);

    const [nameError, setNameError] = useState<string | undefined>(undefined);
    const [descriptionError, setDescriptionError] = useState<string | undefined>(undefined);
    const [relationshipError, setRelationshipError] = useState<string | undefined>(undefined);
    const [mediumError, setMediumError] = useState<string | undefined>(undefined);
    const [durationError, setDurationError] = useState<string | undefined>(undefined);
    const [frequencyError, setFrequencyError] = useState<string | undefined>(undefined);
    const [dateRangeError, setDateRangeError] = useState<string | undefined>(undefined);

    async function onSave() {

        const validationSchema = z.object({

            name: z.string().min(1, "name is required"),

            description: z.string().min(1, "description is required"),

            dateRange: z.object({
                startDate: z.string(),
                endDate: z.string()
            }).optional().refine((data) => {
                if (clientSideState.duration === EngagementBoundedness.FixedTerm) {
                    return data?.startDate !== undefined && data?.endDate !== undefined;
                }
                return true;
            }, "start and end dates are required for fixed term engagements")
                .refine((data) => {
                    if (data?.startDate && data?.endDate) {
                        return data?.startDate <= data?.endDate;
                    }
                    return true;
                }, "start date must be before end date"),

            frequency: z.enum(EngagementFrequency, "frequency is required"),

            relationship: z.enum(RelationshipType, "relationship is required"),

            medium: z.enum(EngagementMedium, "medium is required"),

            duration: z.enum(EngagementBoundedness, "duration is required"),

        })

        const validationResult = validationSchema.safeParse(clientSideState);
        if (!validationResult.success) {

            const errors = z.treeifyError(validationResult.error);
            setNameError(errors.properties?.name?.errors?.[0]);
            setDescriptionError(errors.properties?.description?.errors?.[0]);
            setRelationshipError(errors.properties?.relationship?.errors?.[0]);
            setMediumError(errors.properties?.medium?.errors?.[0]);
            setDurationError(errors.properties?.duration?.errors?.[0]);
            setFrequencyError(errors.properties?.frequency?.errors?.[0]);
            setDateRangeError(errors.properties?.dateRange?.errors?.[0]);
            return;

        } else {

            setNameError(undefined);
            setDescriptionError(undefined);
            setRelationshipError(undefined);
            setMediumError(undefined);
            setDurationError(undefined);
            setFrequencyError(undefined);
            setDateRangeError(undefined);

            const payload = {
                id: clientSideState?.id,
                name: clientSideState.name!,
                description: clientSideState.description!,
                relationshipType: clientSideState.relationship!,
                engagement: {
                    medium: clientSideState.medium!,
                    boundedness: clientSideState.duration!,
                    frequency: clientSideState.frequency!,
                    startDate: clientSideState.dateRange?.startDate,
                    endDate: clientSideState.dateRange?.endDate,
                },
                genders: clientSideState.genders,
                ageRangeMin: clientSideState.ageRange.length > 0 ? clientSideState.ageRange[0] : undefined,
                ageRangeMax: clientSideState.ageRange.length > 1 && clientSideState.ageRange[1] !== maxAge ? clientSideState.ageRange[1] : undefined,
                maximumDistanceKm: clientSideState.maximumDistanceKm === maxDistanceKm ? undefined : clientSideState.maximumDistanceKm,
                active: clientSideState.active
            };

            if (clientSideState.id) {

                const updateSearchProfileResponse = await updateSearchProfile(payload);

                if (!updateSearchProfileResponse.success || !updateSearchProfileResponse.value?.data) {
                    throw new Error("not implemented");
                }

                const updatedSearchProfile = updateSearchProfileResponse.value?.data;
                const newState = {
                    id: updatedSearchProfile.id,
                    name: updatedSearchProfile.name!,
                    description: updatedSearchProfile.description!,
                    relationship: updatedSearchProfile.relationshipType!,
                    medium: updatedSearchProfile.engagement!.medium,
                    duration: updatedSearchProfile.engagement!.boundedness,
                    frequency: updatedSearchProfile.engagement!.frequency,
                    dateRange: updatedSearchProfile.engagement!.startDate && updatedSearchProfile.engagement!.endDate ? {
                        startDate: updatedSearchProfile.engagement!.startDate,
                        endDate: updatedSearchProfile.engagement!.endDate,
                    } : undefined,
                    ageRange: [updatedSearchProfile.ageRangeMin!, updatedSearchProfile.ageRangeMax ?? maxAge] as [number, number | undefined],
                    maximumDistanceKm: updatedSearchProfile.maximumDistanceKm ?? maxDistanceKm,
                    genders: updatedSearchProfile.genders!,
                    active: updatedSearchProfile.active!,
                    createdAt: updatedSearchProfile.createdAt,
                    updatedAt: updatedSearchProfile.updatedAt
                }

                router.refresh();
                setServerSideState(newState);
                setClientSideState(newState);

            } else {

                const createSearchProfileResponse = await createSearchProfile(payload);

                if (!createSearchProfileResponse.success || !createSearchProfileResponse.value?.data) {
                    throw new Error("not implemented");
                }

                router.refresh();
                router.replace(Routes.protected.searchProfile(createSearchProfileResponse.value?.data.id));

            }

        }

    }

    async function onDelete() {
        const deleteResponse = await deleteSearchProfile(serverSideState.id!);
        if (!deleteResponse.success) {
            throw new Error("not implemented");
        }
        router.replace(Routes.protected.searchProfiles);
    }

    function onCancel() {
        setClientSideState(serverSideState);
    }

    async function onSaveNewLocation() {

        const updateProfileResponse = await updateProfile
            (
                user.id,
                user.alias!,
                user.birthDate!,
                user.gender,
                newUserLocation,
                [...(user.images ?? [])].map((image, index) => ({ id: image.id, order: index })),
                user.bio
            );

        if (!updateProfileResponse.success || !updateProfileResponse.value?.data) {
            throw new Error("not implemented");
        }

        setUser(updateProfileResponse.value.data);

    }

    const [newUserLocation, setNewUserLocation] = useState(undefined as Location | undefined);
    const distanceLocationNotSetAlertChildren = (
        <Surface className="flex flex-col p-3 gap-3" variant="secondary">
            <InfoSurface show={true} >
                <p className="text-(--primary-fc) text-(length:--primary-fs)">In order to use the distance filter, you must specify your location.</p>
            </InfoSurface>
            <LocationInput
                label="location"
                value={newUserLocation}
                onChange={(location) => setNewUserLocation(location)}
            />
            <Button intent="save" onClick={() => onSaveNewLocation()}>save</Button>
        </Surface>
    );
    const classNames = [className].filter(Boolean).join(" ");
    return (
        <Surface className={`${classNames} flex flex-col gap-2`} variant="none" padding="none">

            <Surface className="flex flex-col gap-2 grow-1 overflow-y-scroll scrollbar-none" variant="secondary" padding="sm">
                <InfoSurface show={true} >
                    <p>A search-profile is a set of criteria that define the kind of relationship & person you're looking for.</p>
                </InfoSurface>

                <DropDownInput
                    optionClassName="lowercase"
                    errorMessage={relationshipError}
                    label="relationship type"
                    value={enums.enumerate(RelationshipType).find(({ value }) => value === clientSideState.relationship)?.key}
                    placeholder={"relationship type"}
                    options={
                        enums.enumerate(RelationshipType).map(({ key, value }) => ({ label: enums.formatRelationshipType(value).toLocaleLowerCase(), value: key }))
                    }
                    onChange={(relationship) => setClientSideState(prev => ({ ...prev, relationship: enums.parse(RelationshipType, relationship) }))}
                />

                <DropDownInput
                    info="the medium of a relationship describes how you'd like to interact in the relationship: virtually, in-person or a hybrid of the two"
                    infoType="info"
                    optionClassName="lowercase"
                    errorMessage={mediumError}
                    label="medium"
                    value={enums.enumerate(EngagementMedium).find(({ value }) => value === clientSideState.medium)?.key}
                    placeholder={"medium"}
                    options={enums.enumerate(EngagementMedium).map(({ key, value }) => ({ label: enums.formatEngagementMedium(value).toLocaleLowerCase(), value: key }))}
                    onChange={(medium) => setClientSideState(prev => ({ ...prev, medium: enums.parse(EngagementMedium, medium) }))}
                />

                <DropDownInput
                    info="the duration of a relationship describes how long the relationship should last"
                    infoType="info"
                    optionClassName="lowercase"
                    errorMessage={durationError}
                    label="duration"
                    value={enums.enumerate(EngagementBoundedness).find(({ value }) => value === clientSideState.duration)?.key}
                    placeholder="duration"
                    options={enums.enumerate(EngagementBoundedness).map(({ key, value }) => ({ label: enums.formatEngagementBoundedness(value).toLocaleLowerCase(), value: key }))}
                    onChange={(duration) => {
                        setClientSideState(prev => ({ ...prev, duration: enums.parse(EngagementBoundedness, duration) }))
                        if (enums.parse(EngagementBoundedness, duration) === EngagementBoundedness.OneTime) {
                            setClientSideState(prev => ({ ...prev, frequency: EngagementFrequency.Once }));
                        } else {
                            setClientSideState(prev => ({ ...prev, frequency: prev.frequency === EngagementFrequency.Once ? undefined : prev.frequency }))
                        }

                        if (enums.parse(EngagementBoundedness, duration) !== EngagementBoundedness.FixedTerm) {
                            setClientSideState(prev => ({ ...prev, dateRange: undefined }));
                        }
                    }}
                />

                {
                    clientSideState.duration === EngagementBoundedness.FixedTerm &&
                    <>
                        <DateInput
                            value={clientSideState.dateRange?.startDate}
                            onBlur={(value) => setClientSideState(prev => ({ ...prev, dateRange: { ...prev.dateRange, startDate: value } }))}
                            minDate={new Date().toISOString().split("T")[0]}
                            maxDate="2099-01-01"
                            label={"start date"}
                        />
                        <DateInput
                            value={clientSideState.dateRange?.endDate}
                            onBlur={(value) => setClientSideState(prev => ({ ...prev, dateRange: { ...prev.dateRange, endDate: value } }))}
                            minDate={new Date().toISOString().split("T")[0]}
                            maxDate="2099-01-01"
                            label={"end date"}
                        />
                        {
                            dateRangeError && <span className="mx-auto text-(length:--primary-fs) text-(--error-fc)">{dateRangeError}</span>
                        }
                    </>
                }

                {
                    clientSideState.duration !== EngagementBoundedness.OneTime &&
                    <DropDownInput
                        info="the frequency of a relationship describes how often you'd like to interact with the person"
                        infoType="info"
                        optionClassName="lowercase"
                        label="frequency"
                        errorMessage={frequencyError}
                        value={enums.enumerate(EngagementFrequency).find(({ value }) => value === clientSideState.frequency)?.key}
                        placeholder="frequency"
                        options={enums.enumerate(EngagementFrequency).filter(ef => ef.value != EngagementFrequency.Once).map(({ key, value }) => ({ label: enums.formatEngagementFrequency(value).toLocaleLowerCase(), value: key }))}
                        onChange={(frequency) => setClientSideState(prev => ({ ...prev, frequency: enums.parse(EngagementFrequency, frequency) }))}
                    />
                }

                <SelectInput
                    info="specify the genders of the person you'd like to meet (multi-select)"
                    infoType="info"
                    optionClassName="lowercase"
                    multiple={true}
                    value={clientSideState.genders.map(g => enums.enumerate(Gender).find(({ value }) => value === g)!.key)}
                    options={enums.enumerate(Gender).map(({ key, value }) => ({ label: enums.formatGender(value).toLocaleLowerCase(), value: key }))}
                    label={"genders"}
                    onChange={(genders) => {
                        setClientSideState(prev => ({ ...prev, genders: genders.map((value) => enums.parse(Gender, value)!) }))
                    }}
                />

                <DualRangeInput
                    info="specify the age range of the person you'd like to meet"
                    infoType="info"
                    label={"age range"}
                    value={[clientSideState.ageRange[0], clientSideState.ageRange[1] ?? 120]}
                    min={minAge}
                    max={maxAge}
                    step={1}
                    onChange={(ageRange) => setClientSideState(prev => ({ ...prev, ageRange }))}
                />

                <SingleRangeInput
                    info="specify the maximum distance between you and the person you'd like to meet"
                    infoType="info"
                    disabledAlertChildren={distanceLocationNotSetAlertChildren}
                    disabled={user.location === undefined}
                    value={clientSideState.maximumDistanceKm}
                    label={"max distance (km)"}
                    min={minDistance}
                    max={maxDistanceKm}
                    step={1}
                    onChange={(maximumDistanceKm) => setClientSideState(prev => ({ ...prev, maximumDistanceKm }))}
                />

                <TextAreaInput
                    info="describe the relationship you're looking for, we'll use this information to match you with individuals that look for similar things"
                    infoType="info"
                    errorMessage={descriptionError}
                    value={clientSideState.description}
                    label="description"
                    placeholder="describe what you're looking for..."
                    size="lg"
                    onBlur={(description) => setClientSideState(prev => ({ ...prev, description }))}
                />

                <TextInput
                    errorMessage={nameError}
                    value={clientSideState.name}
                    label="name"
                    placeholder="give your search-profile a name..."
                    onBlur={(name) => setClientSideState(prev => ({ ...prev, name }))}
                />

                <CheckboxInput
                    value={clientSideState.active}
                    label={`${clientSideState.active ? "active" : "inactive"}`}
                    onChange={(active) => setClientSideState(prev => ({ ...prev, active }))}
                />

            </Surface>

            <div className="flex flex-row gap-2 justify-between grow-0">
                <Button intent="save" onClick={onSave}>save</Button>
                {
                    serverSideState.id &&
                    <AlertDialog
                        confirmIntent="destructive"
                        title="are you sure?"
                        description="are you sure you want to delete this search-profile?"
                        onCancel={() => { }}
                        onConfirm={onDelete}
                    >
                        <Button intent="destructive">delete</Button>
                    </AlertDialog>
                }
                <Button intent="cancel" onClick={onCancel}>cancel</Button>
            </div>
        </Surface>
    )

}