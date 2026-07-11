import { Mars, Transgender, Venus } from "lucide-react";
import { SearchProfile, User } from "../../types/domain/aggregates"
import { Gender } from "../../types/domain/value-objects";
import ImageGallery from "../images/ImageGallery"
import Surface from "../surfaces/Surface";
import { formatDistance } from "../../distance";

export type ProfileShortInfoProps = {
    user: User,
    searchProfile?: SearchProfile,
    className?: string
}
export function ProfileShortInfo({ user, searchProfile, className }: ProfileShortInfoProps) {

    const classNames = [className].filter(Boolean).join(" ");
    return (
        <Surface className={`${classNames} flex flex-col max-w-full`} padding="sm" variant="tertiary">
            <div className="flex flex-row gap-1 items-center">
                <span className="wrap-anywhere text-(--primary-fc) text-(length:--primary-fs) font-semibold">{user.alias}</span>
                {
                    user.gender === Gender.Male ? <Mars className="w-(--primary-fs) h-(--primary-fs) text-(--primary-fc)" /> :
                        user.gender === Gender.Female ? <Venus className="w-(--primary-fs) h-(--primary-fs) text-(--primary-fc)" /> :
                            user.gender === Gender.TransFemale || user.gender === Gender.TransMale ? <Transgender className="w-(--primary-fs) h-(--primary-fs) text-(--primary-fc)" /> :
                                null
                }
            </div>
            <div className="flex flex-row gap-1">
                {user.age && <span className="text-(--secondary-fc) text-(length:--primary-fs)">{user.age} y.o.</span>}
                {user.distanceKmToQueryingUser !== undefined ? <span className="text-(--secondary-fc) text-(length:--primary-fs)">{formatDistance(user.distanceKmToQueryingUser)}</span> : null}
            </div>
        </Surface>
    )
}

export type ProfileProps = {
    candidate: User,
    candidateSearchProfile?: SearchProfile,
    userSearchProfile?: SearchProfile,
    className?: string
}
export default function Profile({ candidate, candidateSearchProfile, userSearchProfile, className }: ProfileProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ")
    return (
        <Surface className={`w-full flex flex-col gap-2 ${classNames}`} padding="md" variant="secondary">
            {
                userSearchProfile?.name &&
                <Surface className="mx-auto w-full flex flex-col" variant="tertiary" padding="sm">
                    <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold mx-auto">
                        {userSearchProfile.name}
                    </span>
                </Surface>
            }
            <ImageGallery className="h-[300px]" imageUrls={candidate.images?.sort((a, b) => a.imageDetails.order - b.imageDetails.order).map(i => i.imageDetails.url) ?? []} />
            <ProfileShortInfo className="bottom-2 left-2" user={candidate} searchProfile={candidateSearchProfile!} />
            {
                candidate.bio &&
                <div className="flex flex-col gap-1">
                    <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold mx-auto">bio</span>
                    <Surface className="flex flex-col gap-1" padding="sm" variant="tertiary">
                        <p className="wrap-anywhere whitespace-pre-wrap text-(--primary-fc) text-(length:--primary-fs)">{candidate.bio}</p>
                    </Surface>
                </div>
            }
            {
                candidateSearchProfile &&
                <div className="flex flex-col gap-1">
                    <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold mx-auto">looking for</span>
                    <Surface className="flex flex-col gap-1" padding="sm" variant="tertiary">
                        <p className="wrap-anywhere whitespace-pre-wrap text-(--primary-fc) text-(length:--primary-fs)">{candidateSearchProfile.description}</p>
                    </Surface>
                </div>
            }
        </Surface>
    )
}