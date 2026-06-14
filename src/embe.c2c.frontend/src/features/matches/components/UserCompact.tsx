import Surface, { SurfaceProps } from "@/src/shared/components/surfaces/Surface";
import { UserBrief as UserBriefTypeDef } from "../../../shared/types/dtos/types";
import Image from "next/image";
import Link from "next/link";

function shorten(str: string, maxLength: number) {
    if (str.length <= maxLength) {
        return str;
    }
    return str.slice(0, maxLength - 3) + "...";
}

export type UserCompactProps = Omit<SurfaceProps<typeof Link>, "as" | "href" | "children"> & {
    className?: string;
    userBrief?: UserBriefTypeDef;
}
export function UserCompact({ className, userBrief, style, ...props }: UserCompactProps) {
    const classNames = [className].filter(Boolean).join(" ");
    return (
        <Surface as={Link} className={`${classNames} no-underline flex flex-col gap-0 items-center min-w-max`} padding="none" href={`/protected/users/${userBrief?.id}`} style={style} {...props} variant="inherit">
            <span className="max-w-[100px] text-nowrap text-center overflow-hidden text-ellipsis text-(length:--primary-fs)">{userBrief?.userName}</span>
            {
                userBrief?.profilePictureUrl &&
                <Image src={userBrief?.profilePictureUrl} alt="Profile picture" width={0} height={0} className="w-20 h-20 rounded-full object-cover" unoptimized={process.env.NODE_ENV === "development"} />
            }
        </Surface>
    )
}