"use client";

import { InfiniteScroll } from "@/src/shared/components/scroll/infinite-scroll/InfiniteScroll";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { useState } from "react";
import { UserCompact } from "../../matches/components/UserCompact";
import Surface from "@/src/shared/components/surfaces/Surface";
import { getPositiveJudgements } from "../actions";
import { Candidate, CandidatePermission } from "@/src/shared/types/domain/aggregates";

export type LikesProps = {
    initialLikes: ReadDto<Candidate, CandidatePermission>[];
    className?: string;
}
export default function Likes({ initialLikes, className }: LikesProps) {

    const [likes, setLikes] = useState(initialLikes);
    const classNames = [className].filter(Boolean).join(" ");

    const page = likes.length > 0 ? 2 : 1;
    const size = likes.length > 0 ? likes.length : 50;

    const items = likes.map(like => {

        return (

            <li key={like.data.id}>
                <Surface className="flex gap-2 items-center" variant="secondary" padding="sm">
                    <UserCompact dto={like.data.user} />
                    {
                        like.data.updatedAt &&
                        <span className="text-(--primary-fc) text-(length:--primary-fs) mx-auto">liked you on {new Date(like.data.updatedAt).toLocaleDateString()}</span>
                    }
                </Surface>
            </li>

        )

    });

    async function loadMore(): Promise<boolean> {

        const response = await getPositiveJudgements(page, size);
        if (!response.success) {
            throw new Error("not implemented");
        }

        const newLikes = response.value!;
        setLikes(prev => [...prev, ...newLikes]);

        return response.value!.length == size;

    }

    return (
        <>
            {
                likes.length > 0 ?
                    < InfiniteScroll
                        className={`${classNames} flex flex-col gap-2`
                        }
                        direction="down"
                        callback={loadMore} >
                        {items}
                    </InfiniteScroll >
                    : <span className={`${classNames} text-(--primary-fc) text-(length:--primary-fs) flex flex-col items-center justify-center`}><strong>no likes yet</strong></span>
            }
        </>
    )

}