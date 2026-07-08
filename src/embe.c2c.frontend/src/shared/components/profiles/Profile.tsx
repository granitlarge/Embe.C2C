import { useEffect, useState } from "react";
import { calculateAge } from "../../time";
import { User } from "../../types/domain/aggregates"
import ImageGallery from "../images/ImageGallery"
import Surface from "../surfaces/Surface";
import { reverseGeocode } from "../../actions/geography/actions";

function ProfileShortInfo({ user, className }: { user: User, className?: string }) {

    const classNames = [className].filter(Boolean).join(" ");
    const [locationName, setLocationName] = useState<string | undefined>(undefined);

    return (
        <Surface className={`${classNames} flex flex-col max-w-full`} padding="sm" variant="tertiary">
            <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">{user.alias}</span>
            <span className="text-(--secondary-fc) text-(length:--secondary-fs)">{calculateAge(user.birthDate ?? "2000-01-01")} years old</span>
            {user.distanceKm !== undefined ? <span className="text-(--secondary-fc) text-(length:--secondary-fs)">{user.distanceKm} km away</span> : null}
        </Surface>
    )

}

export type ProfileProps = {
    user: User
}
export default function Profile({ user }: ProfileProps) {
    return (
        <Surface className="flex flex-col gap-2" padding="none" variant="secondary">
            <ImageGallery imageUrls={user.images?.map(i => i.imageDetails.url) ?? []} />
            <ProfileShortInfo className="bottom-2 left-2" user={user} />
        </Surface>
    )
}