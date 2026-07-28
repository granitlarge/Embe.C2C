import Link from "@/src/shared/components/Links/Link";
import Surface, { SurfaceProps } from "@/src/shared/components/surfaces/Surface";
import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { User as UserIcon, } from "@deemlol/next-icons";
import Image from "@/src/shared/components/images/Image";
import { Routes } from "@/src/shared/routes";

export type UserCompactProps = Omit<SurfaceProps<typeof Link>, "as" | "href" | "children"> & {
    className?: string;
    dto?: ReadDto<User, UserPermission>;
}
export function UserCompact({ className, dto, ...props }: UserCompactProps) {
    const classNames = [className].filter(Boolean).join(" ");
    const user = dto?.data;
    const profilePicture = [...(dto?.data?.images ?? [])]
        .sort((a, b) => a.imageDetails.order - b.imageDetails.order)
        .at(0);

    return (
        <>
            {
                user?.id &&
                <Surface as={Link} className={`${classNames} no-underline flex flex-col gap-0 items-center min-w-max`} padding="none" href={Routes.protected.user(user?.id)} {...props} variant="inherit">
                    {
                        profilePicture?.imageDetails?.url &&
                            <Image 
                                src={profilePicture?.imageDetails?.smallUrl ?? profilePicture?.imageDetails?.mediumUrl ?? profilePicture.imageDetails?.largeUrl ?? profilePicture?.imageDetails?.url} 
                                alt="Profile picture" 
                                width={100}
                                height={100} 
                                className="w-20 h-20 rounded-full object-cover" 
                                unoptimized={process.env.NODE_ENV === "development"} 
                            />
                    }
                    {
                        !profilePicture?.imageDetails?.url &&
                        <UserIcon className="w-20 h-20 rounded-full bg-transparent flex items-center justify-center text-(--primary-fc) text-(length:--primary-fs)" />
                    }
                    {user?.alias && <span className="max-w-[100px] text-nowrap text-center overflow-hidden text-ellipsis text-(--primary-fc) text-(length:--primary-fs) font-bold">{user?.alias}</span>}
                </Surface>
            }
        </>
    )
}