"use client";

import { RefreshCcw, Trash2 } from "lucide-react";
import { Location } from "@/src/shared/types/domain/value-objects";
import * as Tabs from "@radix-ui/react-tabs";
import { useEffect, useState } from "react";
import DropDownInput from "../dropdown-input/DropDownInput";
import { AdminArea } from "@/src/shared/actions/geography/types";
import { getAdminAreaById, getCountryAdminAreas, reverseGeocode, searchAdminAreas } from "@/src/shared/actions/geography/actions";
import { Loader } from "@deemlol/next-icons";
import Surface from "../../surfaces/Surface";

type LocationInputExactProps = {
    value?: Location;
    onChange?: (value?: Location) => void;
    className?: string;
}
function LocationInputExact({ value, onChange, className }: LocationInputExactProps) {

    const [loading, setLoading] = useState(false);
    const [locationName, setLocationName] = useState<string | undefined>(undefined);
    const classNames = [
        className
    ].filter(Boolean).join(" ");

    useEffect(() => {

        async function loadLocationName() {
            const response = await reverseGeocode(value!.longitude, value!.latitude);
            if (!response.success) {
                throw new Error("not implemented");
            }
            const adminAreas = response.value!;
            if (adminAreas.length === 0) {
                throw new Error("not implemented");
            }
            adminAreas.sort((a, b) => a.level - b.level);
            setLocationName(adminAreas.map(a => a.name).join(", "));
        }

        if (value) {
            loadLocationName();
        } else {
            setLocationName(undefined);
        }

    }, [value]);

    function updateLocation() {

        setLoading(true);
        window.navigator.geolocation.getCurrentPosition(async (position) => {

            const { latitude, longitude } = position.coords;
            onChange?.({ latitude, longitude });
            setLoading(false);

        }, (error) => {

            setLoading(false);
            throw new Error("not implemented");

        });

    }

    return (
        <Surface className={`input-wrapper ${classNames}`} variant="inherit" padding="sm">
            <div className="flex flex-row items-center">
                <input className="overflow-x-scroll" type="text" disabled value={locationName ? locationName : value ? `${value.latitude}, ${value.longitude}` : "location not set"} />
                <button className="max-w-max bg-transparent" onClick={updateLocation}>
                    <RefreshCcw className={`w-(--primary-fs) h-(--primary-fs) text-(--primary-fc) ${loading ? "animate-[spin_1s_linear_infinite_reverse]" : ""}`} />
                </button>
                <button className="max-w-max bg-transparent" onClick={() => {onChange?.(undefined)}}>
                    <Trash2 className={`w-(--primary-fs) h-(--primary-fs) text-(--primary-fc)`} />
                </button>
            </div>
        </Surface>
    )

}

type LocationInputApproximateProps = {
    value?: Location;
    onChange?: (value: Location) => void;
    className?: string;
}
function LocationInputApproximate({ value, onChange, className }: LocationInputApproximateProps) {

    const [loading, setLoading] = useState(true);
    const [countries, setCountries] = useState<AdminArea[]>([]);
    const [level1AdminAreas, setLevel1AdminAreas] = useState<AdminArea[]>([]);
    const [level2AdminAreas, setLevel2AdminAreas] = useState<AdminArea[]>([]);
    const [level3AdminAreas, setLevel3AdminAreas] = useState<AdminArea[]>([]);
    const [level4AdminAreas, setLevel4AdminAreas] = useState<AdminArea[]>([]);
    const [level5AdminAreas, setLevel5AdminAreas] = useState<AdminArea[]>([]);

    const [selectedCountry, setSelectedCountry] = useState<AdminArea | undefined>(undefined);
    const [selectedLevel1AdminArea, setSelectedLevel1AdminArea] = useState<AdminArea | undefined>(undefined);
    const [selectedLevel2AdminArea, setSelectedLevel2AdminArea] = useState<AdminArea | undefined>(undefined);
    const [selectedLevel3AdminArea, setSelectedLevel3AdminArea] = useState<AdminArea | undefined>(undefined);
    const [selectedLevel4AdminArea, setSelectedLevel4AdminArea] = useState<AdminArea | undefined>(undefined);
    const [selectedLevel5AdminArea, setSelectedLevel5AdminArea] = useState<AdminArea | undefined>(undefined);

    useEffect(() => {

        async function load() {

            setLoading(true);

            if (value) {

                const response = await searchAdminAreas(undefined, value.longitude, value.latitude, 1, 50);
                if (!response.success) {
                    throw new Error("not implemented");
                }

                const adminAreas = response.value!;
                if (adminAreas.length === 0) {
                    throw new Error("not implemented");
                }

                const adminArea = adminAreas[0];
                await loadAndSetAdminAreasRecursively(adminArea);
                if (adminArea.level < 5) {
                    await loadNextLevelAdminAreas(adminArea.id, adminArea.level);
                }

            } else {

                const response = await getCountryAdminAreas();
                if (!response.success) {
                    throw new Error("not implemented");
                }
                setCountries(response.value!.sort((a, b) => a.name.localeCompare(b.name)));

            }

            setLoading(false);

        }

        load();

    }, [value]);

    async function loadAndSetAdminAreasRecursively(leafAdminArea: AdminArea) {

        if (leafAdminArea.parentId === undefined) {
            setSelectedCountry(leafAdminArea);
            if (countries !== undefined && countries.length > 0) {
                return;
            }
            const newCountries = await getCountryAdminAreas();
            if (!newCountries.success) {
                throw new Error("not implemented");
            }
            setCountries(newCountries.value!.sort((a, b) => a.name.localeCompare(b.name)));
            return;
        }

        const response = await searchAdminAreas(leafAdminArea.parentId, undefined, undefined, 1, 1_000);
        if (!response.success)
            throw new Error("not implemented");

        setAdminAreas(response.value!, leafAdminArea.level);
        selectAdminArea(leafAdminArea);
        const parent = await getAdminAreaById(leafAdminArea.parentId!);
        await loadAndSetAdminAreasRecursively(parent.value!);

    }

    async function loadNextLevelAdminAreas(parentId: string, level: number) {

        setLoading(true);

        const response = await searchAdminAreas(parentId, undefined, undefined, 1, 1_000);
        if (!response.success) {
            throw new Error("not implemented");
        }

        const adminAreas = response.value!;
        setAdminAreas(adminAreas, level + 1);

        setLoading(false);

    }

    async function onCountrySelected(countryId: string) {

        const country = countries.find(c => c.id === countryId);
        selectAdminArea(country!);
        clearLowerLevelSelections(0);
        onChange?.({ latitude: country!.latitude, longitude: country!.longitude });
        await loadNextLevelAdminAreas(countryId, 0);

    }

    async function onAdminAreaSelected(adminAreaId: string, level: number) {

        const adminArea = findAdminAreaByIdAndLevel(adminAreaId, level);
        if (adminArea === undefined) {
            throw new Error("not implemented");
        }

        selectAdminArea(adminArea);
        clearLowerLevelSelections(level);
        onChange?.({ latitude: adminArea.latitude, longitude: adminArea.longitude });
        await loadNextLevelAdminAreas(adminAreaId, level);
    }

    function findAdminAreaByIdAndLevel(adminAreaId: string, level: number): AdminArea | undefined {
        switch (level) {
            case 0:
                return countries.find(c => c.id === adminAreaId);
            case 1:
                return level1AdminAreas.find(c => c.id === adminAreaId);
            case 2:
                return level2AdminAreas.find(c => c.id === adminAreaId);
            case 3:
                return level3AdminAreas.find(c => c.id === adminAreaId);
            case 4:
                return level4AdminAreas.find(c => c.id === adminAreaId);
            case 5:
                return level5AdminAreas.find(c => c.id === adminAreaId);
        }
    }


    function setAdminAreas(adminAreas: AdminArea[], level: number) {

        adminAreas.sort((a, b) => a.name.localeCompare(b.name));
        switch (level) {
            case 0:
                setCountries(adminAreas);
                break;
            case 1:
                setLevel1AdminAreas(adminAreas);
                break;
            case 2:
                setLevel2AdminAreas(adminAreas);
                break;
            case 3:
                setLevel3AdminAreas(adminAreas);
                break;
            case 4:
                setLevel4AdminAreas(adminAreas);
                break;
            case 5:
                setLevel5AdminAreas(adminAreas);
                break;
        }

    }

    function selectAdminArea(adminArea: AdminArea) {
        switch (adminArea.level) {
            case 0:
                setSelectedCountry(adminArea);
                break;
            case 1:
                setSelectedLevel1AdminArea(adminArea);
                break;
            case 2:
                setSelectedLevel2AdminArea(adminArea);
                break;
            case 3:
                setSelectedLevel3AdminArea(adminArea);
                break;
            case 4:
                setSelectedLevel4AdminArea(adminArea);
                break;
            case 5:
                setSelectedLevel5AdminArea(adminArea);
                break;
        }
    }

    function clearLowerLevelSelections(level: number) {

        switch (level) {
            case 0:
                setSelectedLevel1AdminArea(undefined);
            case 1:
                setSelectedLevel2AdminArea(undefined);
            case 2:
                setSelectedLevel3AdminArea(undefined);
            case 3:
                setSelectedLevel4AdminArea(undefined);
            case 4:
                setSelectedLevel5AdminArea(undefined);
                break;
        }

    }

    // 1. no value
    // * fetch all countries
    // * as the user selects a country, fetch all level1 locations for that country, and so on
    // 2. has value
    // * fetch the AdminArea for the location. If there are multiple adminareas with the same coordinates, take the highest level one.
    // * fetch the adminArea of the parent, do so until you've reached the country level.
    // * then, for each level, fetch all the admin areas for that level, and set the value of the dropdown to the adminArea that matches the location's coordinates.

    const dropDowns = (
        <>
            {
                < DropDownInput
                    label="country"
                    placeholder="select country"
                    value={selectedCountry?.id}
                    options={countries.map(c => ({ value: c.id, label: c.name }))}
                    onChange={onCountrySelected}
                />
            }
            {
                selectedCountry &&
                level1AdminAreas.length > 0 &&
                <DropDownInput
                    label={level1AdminAreas[0].engType.toLocaleLowerCase()}
                    value={selectedLevel1AdminArea?.id}
                    placeholder={`select ${level1AdminAreas[0].engType?.toLocaleLowerCase()} (optional)`}
                    options={level1AdminAreas.map(c => ({ value: c.id, label: c.name }))}
                    onChange={(value) => onAdminAreaSelected(value, 1)}
                />
            }
            {
                selectedLevel1AdminArea &&
                level2AdminAreas.length > 0 &&
                <DropDownInput
                    label={level2AdminAreas[0].engType.toLocaleLowerCase()}
                    value={selectedLevel2AdminArea?.id}
                    placeholder={`select ${level2AdminAreas[0].engType.toLocaleLowerCase()} (optional)`}
                    options={level2AdminAreas.map(c => ({ value: c.id, label: c.name }))}
                    onChange={(value) => onAdminAreaSelected(value, 2)}
                />
            }
            {
                selectedLevel2AdminArea &&
                level3AdminAreas.length > 0 &&
                <DropDownInput
                    label={level3AdminAreas[0].engType.toLocaleLowerCase()}
                    value={selectedLevel3AdminArea?.id}
                    placeholder={`select ${level3AdminAreas[0].engType.toLocaleLowerCase()} (optional)`}
                    options={level3AdminAreas.map(c => ({ value: c.id, label: c.name }))}
                    onChange={(value) => onAdminAreaSelected(value, 3)}
                />
            }
            {
                selectedLevel3AdminArea &&
                level4AdminAreas.length > 0 &&
                <DropDownInput
                    label={level4AdminAreas[0].engType.toLocaleLowerCase()}
                    value={selectedLevel4AdminArea?.id}
                    placeholder={`select ${level4AdminAreas[0].engType.toLocaleLowerCase()} (optional)`}
                    options={level4AdminAreas.map(c => ({ value: c.id, label: c.name }))}
                    onChange={(value) => onAdminAreaSelected(value, 4)}
                />
            }
            {
                selectedLevel4AdminArea &&
                level5AdminAreas.length > 0 &&
                <DropDownInput
                    label={level5AdminAreas[0].engType.toLocaleLowerCase()}
                    value={selectedLevel5AdminArea?.id}
                    placeholder={`select ${level5AdminAreas[0].engType.toLocaleLowerCase()} (optional)`}
                    options={level5AdminAreas.map(c => ({ value: c.id, label: c.name }))}
                    onChange={(value) => onAdminAreaSelected(value, 5)}
                />
            }
        </>
    );
    return (
        <Surface className="flex flex-col gap-2 w-full" variant="inherit" padding="sm">
            {
                !loading && dropDowns
            }
            {
                loading && <Loader className="mx-auto animate-spin" />
            }
        </Surface>
    )

}

export type LocationInputProps = {
    errorMessage?: string;
    value?: Location;
    onChange?: (value?: Location) => void;
    className?: string;
}
export default function LocationInput({ errorMessage, value, onChange, className }: LocationInputProps) {

    const [tab, setTab] = useState<"exact" | "approximate">("exact");

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const exactTabButtonClassNames = [
        tab === "exact" ? "border-b-3 border-solid border-(--border-color)" : "",
    ].filter(Boolean).join(" ");

    const approximateTabButtonClassNames = [
        tab === "approximate" ? "border-b-3 border-solid border-(--border-color)" : "",
    ].filter(Boolean).join(" ");

    return (

        <div className="flex flex-col gap-2 w-full">
            <span className="mx-auto label">location</span>
            <Tabs.Root value={tab} onValueChange={(value) => setTab(value as "exact" | "approximate")} className={`${classNames} w-full flex flex-col gap-3`}>
                <Tabs.List className="flex flex-row gap-0 w-full">
                    <Tabs.Trigger value="exact" className={`w-full rounded-none rounded-l-md ${exactTabButtonClassNames}`} >
                        exact
                    </Tabs.Trigger>
                    <Tabs.Trigger value="approximate" className={`w-full rounded-none rounded-r-md ${approximateTabButtonClassNames}`} >
                        approximate
                    </Tabs.Trigger>
                </Tabs.List>
                <Tabs.Content value="exact" className={``}>
                    <LocationInputExact value={value} onChange={onChange} className={classNames} />
                </Tabs.Content>
                <Tabs.Content value="approximate" className={``}>
                    <LocationInputApproximate value={value} onChange={onChange} className={classNames} />
                </Tabs.Content>
            </Tabs.Root>
        </div>

    )

}