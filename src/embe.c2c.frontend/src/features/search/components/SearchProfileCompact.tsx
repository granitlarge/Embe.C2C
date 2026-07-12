"use client";

import Surface from "@/src/shared/components/surfaces/Surface";
import { SearchProfile, SearchProfilePermission } from "@/src/shared/types/domain/aggregates"
import { ReadDto } from "@/src/shared/types/dtos/types";
import * as enums from "@/src/shared/enums";
import { EngagementBoundedness, EngagementMedium, Gender, RelationshipType } from "@/src/shared/types/domain/value-objects";
import { ArrowRight, Calendar, Clock, Globe, Handshake, Heart, Mars, Radius, Transgender, Users, Venus } from "lucide-react";
import { useRouter } from "nextjs-toploader/app";

function Chip({ children }: { children: React.ReactNode }) {
    return (
        <Surface className="flex gap-1 items-center p-1" variant="primary" padding="none">
            {children}
        </Surface>
    )
}
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

    const dateStart = searchProfile.engagement?.boundedness === EngagementBoundedness.FixedTerm ? searchProfile.engagement?.startDate : undefined;
    const dateEnd = searchProfile.engagement?.boundedness === EngagementBoundedness.FixedTerm ? searchProfile.engagement?.endDate : undefined;

    const genders = searchProfile.genders ?? [];
    const ageRange = searchProfile.ageRangeMin !== undefined ?
        searchProfile.ageRangeMax !== undefined ? `${searchProfile.ageRangeMin}-${searchProfile.ageRangeMax}` : `${searchProfile.ageRangeMin}+` : undefined;
    const distance = searchProfile.maximumDistanceKm !== undefined ? `${searchProfile.maximumDistanceKm} km` : undefined;
    const active = searchProfile.active;

    return (
        <Surface variant="tertiary" padding="md" className="relative w-full flex flex-col gap-3" onClick={() => {
            router.push(`/protected/search-profile/${searchProfile.id}`);
        }}>
            <>
                {
                    active !== undefined && active &&
                    <span className={`absolute right-3 top-3 w-2 h-2 rounded-full bg-(--active-color) mb-auto`} />
                }
                {
                    active !== undefined && !active &&
                    <span className={`absolute right-3 top-3 w-2 h-2 rounded-full bg-(--inactive-color) mb-auto`} />
                }
                <div className="flex flex-col items-center">
                    <h3>{searchProfile.name}</h3>
                </div>
                <div className="flex flex-row gap-2 flex-wrap justify-start items-center">
                    {
                        relationshipType !== undefined &&
                        <Chip>
                            {
                                relationshipType === RelationshipType.Romantic && <Heart className="w-(--primary-fs) h-(--primary-fs)" /> ||
                                relationshipType === RelationshipType.Platonic && <Users className="w-(--primary-fs) h-(--primary-fs)" /> ||
                                relationshipType === RelationshipType.Professional && <Handshake className="w-(--primary-fs) h-(--primary-fs)" />
                            }
                            <span className="text-(--primary-fc) text-(length:--primary-fs) lowercase">
                                {enums.formatRelationshipType(relationshipType)}
                            </span>
                        </Chip>
                    }
                    {
                        medium !== undefined &&
                        <Chip>
                            {
                                medium === EngagementMedium.Virtual && <Globe className="w-(--primary-fs) h-(--primary-fs)" /> ||
                                medium === EngagementMedium.InPerson && <Users className="w-(--primary-fs) h-(--primary-fs)" /> 
                            }
                            <span className="text-(--primary-fc) text-(length:--primary-fs) lowercase">
                                {enums.formatEngagementMedium(medium)}
                            </span>
                        </Chip>
                    }
                    {
                        boundedness !== undefined && boundedness !== EngagementBoundedness.FixedTerm &&
                        <Chip>
                            <Clock className="w-(--primary-fs) h-(--primary-fs)" />
                            <span className="text-(--primary-fc) text-(length:--primary-fs) lowercase">
                                {enums.formatEngagementBoundedness(boundedness)}
                            </span>
                        </Chip>
                    }
                    {
                        dateStart && dateEnd &&
                        <Chip>
                            <span className="text-(--primary-fc) text-(length:--primary-fs)">{dateStart}</span>
                            <ArrowRight className="w-(--primary-fs) h-(--primary-fs)" />
                            <span className="text-(--primary-fc) text-(length:--primary-fs)">{dateEnd}</span>
                        </Chip>
                    }
                    {
                        frequency !== undefined && boundedness !== EngagementBoundedness.OneTime &&
                        <Chip>
                            <Calendar className="w-(--primary-fs) h-(--primary-fs)" />
                            <span className="text-(--primary-fc) text-(length:--primary-fs) lowercase">{enums.formatEngagementFrequency(frequency)}</span>
                        </Chip>
                    }
                    {
                        ageRange &&
                        <Chip>
                            <span className="text-(--primary-fc) text-(length:--primary-fs)">{ageRange}</span>
                        </Chip>
                    }
                    {
                        distance &&
                        <Chip>
                            <Radius className="w-(--primary-fs) h-(--primary-fs)" />
                            <span className="text-(--primary-fc) text-(length:--primary-fs)">{distance}</span>
                        </Chip>
                    }
                    <Chip>
                        {
                            genders.some(gender => gender === Gender.Male) && <Mars className="w-5 h-5" />
                        }
                        {
                            genders.some(gender => gender === Gender.Female) && <Venus className="w-5 h-5" />
                        }
                        {
                            genders.some(gender => gender === Gender.TransFemale || gender === Gender.TransMale) && <Transgender className="w-5 h-5" />
                        }
                    </Chip>
                </div>
            </>
        </Surface>
    )

}