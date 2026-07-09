import { Mars, Transgender, Venus } from "lucide-react";
import { User } from "../../types/domain/aggregates"
import { Gender } from "../../types/domain/value-objects";
import ImageGallery from "../images/ImageGallery"
import Surface from "../surfaces/Surface";
import { formatDistance } from "../../distance";

function ProfileShortInfo({ user, className }: { user: User, className?: string }) {

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
            {user.age && <span className="text-(--secondary-fc) text-(length:--primary-fs)">{user.age} y.o.</span>}
            {user.distanceKmToQueryingUser !== undefined ? <span className="text-(--secondary-fc) text-(length:--primary-fs)">{formatDistance(user.distanceKmToQueryingUser)}</span> : null}
        </Surface>
    )

}

export type ProfileProps = {
    user: User,
    className?: string
}
export default function Profile({ user, className}: ProfileProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ")
    return (
        <Surface className={`flex flex-col gap-2 ${classNames}`} padding="none" variant="secondary">
            <ImageGallery imageUrls={user.images?.map(i => i.imageDetails.url) ?? []} />
            <ProfileShortInfo className="bottom-2 left-2" user={user} />
            {
                user.bio &&
                <div className="flex flex-col gap-1">
                    <Surface className="flex flex-col gap-1" padding="sm" variant="tertiary">
                        <p className="wrap-anywhere text-(--primary-fc) text-(length:--primary-fs)">{user.bio}</p>
                    </Surface>
                </div>
            }
        </Surface>
    )
}