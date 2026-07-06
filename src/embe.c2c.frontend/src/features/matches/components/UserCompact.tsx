import Surface, { SurfaceProps } from "@/src/shared/components/surfaces/Surface";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
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
    dto?: ReadDto<User, UserPermission>;
}
export function UserCompact({ className, dto, style, ...props }: UserCompactProps) {
    const classNames = [className].filter(Boolean).join(" ");
    const user = dto?.data;
    return (
        <>
            {
                user?.id &&
                <Surface as={Link} className={`${classNames} no-underline flex flex-col gap-0 items-center min-w-max`} padding="none" href={`/protected/users/${user?.id}`} style={style} {...props} variant="inherit">
                    {user?.userName && <span className="max-w-[100px] text-nowrap text-center overflow-hidden text-ellipsis text-(--primary-fc) text-(length:--primary-fs)">{user?.userName}</span>}
                    {
                        user?.profilePicture?.imageDetails?.url &&
                        <Image src={user?.profilePicture?.imageDetails?.url} alt="Profile picture" width={0} height={0} className="w-20 h-20 rounded-full object-cover" unoptimized={process.env.NODE_ENV === "development"} />
                    }
                </Surface>
            }
        </>
    )
}