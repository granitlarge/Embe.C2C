"use client";

import { RefreshCcw } from "lucide-react";
import { Location } from "@/src/shared/types/domain/value-objects";

export type LocationInputProps = {
    errorMessage?: string;
    value?: Location;
    onChange?: (value: Location) => void;
    className?: string;
}
export default function LocationInput({ errorMessage, value, onChange, className }: LocationInputProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    function updateLocation() {

        window.navigator.geolocation.getCurrentPosition((position) => {

            const { latitude, longitude } = position.coords;
            onChange?.({ latitude, longitude });

        }, (error) => {

            console.error("Error getting location:", error);

        });

    }

    return (
        <div className={`input-wrapper ${classNames}`}>
            <span className="label">location</span>
            <div className="relative">
                <input type="text" className="px-10" disabled value={value ? `${value.latitude}, ${value.longitude}` : "location not set"} />
                <button className="absolute right-2 top-1/2 transform -translate-y-1/2 max-w-max bg-transparent" onClick={updateLocation}>
                    <RefreshCcw className=" w-(--primary-fs) h-(--primary-fs) text-(--primary-fc)" />
                </button>
            </div>
        </div>
    )

}