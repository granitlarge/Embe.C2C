import { UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { User as UserTypeDef } from "@/src/shared/types/domain/aggregates";
import Surface from "@/src/shared/components/surfaces/Surface";
import ImageGallery from "@/src/shared/components/image-gallery/ImageGallery";

export type FindUserDatingProps = {
    dto: ReadDto<UserTypeDef, UserPermission>;
    className?: string;
}
export default function FindUserDating({ dto, className }: FindUserDatingProps) {

    const user = dto.data;
    const permissions = dto.permissions;

    const classNames = [className].filter(Boolean).join(" ");
    return (
        <Surface variant="inherit" className={`flex flex-col ${classNames}`}>
            {
                user.files && user.files.length > 0 &&
                <ImageGallery imageUrls={user.files.map(file => file.fileDetails.url)} />
            }
            {user.userName && <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">{user.userName}</span>}
            {user.age && <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">{user.age}</span>}
            {user.gender && <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">{user.gender}</span>}
        </Surface>
    )

}