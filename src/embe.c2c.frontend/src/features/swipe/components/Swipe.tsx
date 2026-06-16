import { User, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";

export type SwipeProps = {
    candidates: ReadDto<User, UserPermission>[];
}

export default function Swipe({ candidates }: SwipeProps) {
    return (
        <>
        </>
    )
}