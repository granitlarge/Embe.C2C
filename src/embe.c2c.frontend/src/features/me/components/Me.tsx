import Surface from "@/src/shared/components/surfaces/Surface"
import MyBasicInfoForm from "./MyBasicInfoForm"
import { User, UserPermission } from "@/src/shared/types/domain/aggregates"
import { ReadDto } from "@/src/shared/types/dtos/types"

export type MeProps = {
    className?: string,
    user: ReadDto<User, UserPermission>
}

export default function Me({ className, user }: MeProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ")

    return (

        // Information
        // ------------
        // Images
        // Alias
        // BirthDate
        // Gender
        // Location

        <Surface className={`${classNames}`} padding="none">
            <MyBasicInfoForm user={user} />
        </Surface>

    )

}