"use client";

import Surface from "@/src/shared/components/surfaces/Surface";
import { SearchProfile, SearchProfilePermission } from "@/src/shared/types/domain/aggregates"
import { ReadDto } from "@/src/shared/types/dtos/types";
import * as enums from "@/src/shared/enums";
import { EngagementBoundedness, Gender } from "@/src/shared/types/domain/value-objects";
import { Mars, Transgender, Venus } from "lucide-react";
import { useRouter } from "next/navigation";

export type SearchProfileCompactProps = {
    searchProfile: ReadDto<SearchProfile, SearchProfilePermission>;
}
export default function SearchProfileCompact({ searchProfile: searchProfileDto }: SearchProfileCompactProps) {

    const router = useRouter();
    const searchProfile = searchProfileDto.data;

    const relationshipType = searchProfile.relationshipType;
    const medium = searchProfile.engagement?.medium;
    const boundedness = searchProfile.engagement?.boundedness;
    const frequency = searchProfile.engagement?.frequency;

    const dateRange = searchProfile.engagement?.boundedness === EngagementBoundedness.FixedTerm &&
        searchProfile.engagement.startDate && searchProfile.engagement.endDate
        ? `${searchProfile.engagement.startDate} - ${searchProfile.engagement.endDate}`
        : undefined;

    const genders = searchProfile.genders ?? [];
    const ageRange = searchProfile.ageRangeMin !== undefined ?
        searchProfile.ageRangeMax !== undefined ? `${searchProfile.ageRangeMin}-${searchProfile.ageRangeMax}` : `${searchProfile.ageRangeMin}+` : undefined;
    const distance = searchProfile.maximumDistanceKm !== undefined ? `<=${searchProfile.maximumDistanceKm} km` : undefined;
    const active = searchProfile.active;

    return (
        <Surface variant="tertiary" padding="md" className="relative w-full flex flex-col gap-3" onClick={() => {
            router.push(`/protected/search-profile/${searchProfile.id}`);
        }}>
            <>
                {
                    active !== undefined && active &&
                    <span className={`absolute right-1 top-1 w-2 h-2 rounded-full bg-(--active-color) mb-auto`} />
                }
                {
                    active !== undefined && !active &&
                    <span className={`absolute right-1 top-1 w-2 h-2 rounded-full bg-(--inactive-color) mb-auto`} />
                }
                <div className="flex flex-col items-center">
                    <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold mx-auto">{searchProfile.name}</span>
                    <div className="flex flex-row items-center gap-1">
                        {relationshipType !== undefined && <span className="text-(--secondary-fc) text-(length:--primary-fs) lowercase">{enums.formatRelationshipType(relationshipType)}</span>}
                        <div className="flex flex-row gap-0 items-center">
                            {
                                genders.some(gender => gender === Gender.Male) && <Mars className="w-(--primary-fs) h-(--primary-fs)" />
                            }
                            {
                                genders.some(gender => gender === Gender.Female) && <Venus className="w-(--primary-fs) h-(--primary-fs)" />
                            }
                            {
                                genders.some(gender => gender === Gender.TransFemale || gender === Gender.TransMale) && <Transgender className="w-(--primary-fs) h-(--primary-fs)" />
                            }
                        </div>
                    </div>
                </div>
                <div className="flex flex-row gap-10 justify-between">
                    <ul className="flex flex-col gap-2">
                        {
                            medium !== undefined &&
                            <li className="list-disc list-inside">
                                <span className="text-(--secondary-fc) text-(length:--primary-fs) lowercase">
                                    {enums.formatEngagementMedium(medium)}
                                </span>
                            </li>
                        }
                        {
                            boundedness !== undefined && boundedness !== EngagementBoundedness.FixedTerm &&
                            <li className="list-disc list-inside">
                                <span className="text-(--secondary-fc) text-(length:--primary-fs) lowercase">
                                    {enums.formatEngagementBoundedness(boundedness)}
                                </span>
                            </li>
                        }
                        {
                            frequency !== undefined && boundedness !== EngagementBoundedness.OneTime &&
                            <li className="list-disc list-inside">
                                <span className="text-(--secondary-fc) text-(length:--primary-fs) lowercase">{enums.formatEngagementFrequency(frequency)}</span>
                            </li>
                        }
                        {
                            dateRange &&
                            <li className="list-disc list-inside">
                                <span className="text-(--secondary-fc) text-(length:--primary-fs)">{dateRange}</span>
                            </li>
                        }
                    </ul>
                    <div className="flex flex-col gap-2">

                        {
                            ageRange &&
                            <li className="list-disc list-inside">
                                <span className="text-(--secondary-fc) text-(length:--primary-fs)">{ageRange}</span>
                            </li>
                        }
                        {
                            distance &&
                            <li className="list-disc list-inside">
                                <span className="text-(--secondary-fc) text-(length:--primary-fs)">{distance}</span>
                            </li>
                        }

                    </div>
                </div>
            </>
        </Surface>
    )

}