import Surface, { SurfaceProps } from "@/src/shared/components/surfaces/Surface";
import { UserBrief as UserBriefTypeDef } from "../../../shared/types/dtos/types";
import Image from "next/image";
import Link from "next/link";

export type UserCompactProps = Omit<SurfaceProps<typeof Link>, "as" | "href" | "children"> & {
    className?: string;
    userBrief?: UserBriefTypeDef;
}

export function UserCompact({ className, userBrief, ...props }: UserCompactProps) {
    return (
        <Surface as={Link} className={`${className} no-underline flex flex-col gap-0 items-center`} padding="none" href={`/protected/users/${userBrief?.id}`} {...props}>
            <span className="text-(length:--fs-7)">{userBrief?.userName}</span>
            {
                userBrief?.profilePictureUrl &&
                <Image src={userBrief?.profilePictureUrl} alt="Profile picture" width={0} height={0} className="w-20 h-20 rounded-full object-cover" unoptimized={process.env.NODE_ENV === "development"} />
            }
        </Surface>
    )
}