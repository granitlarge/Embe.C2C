"use client";

import { AuthenticatedUser } from "@/src/shared/user";
import { MatchCompact } from "./MatchCompact";
import { Matching, MatchingPermission } from "@/src/shared/types/domain/aggregates";
import { useState } from "react";
import { InfiniteScroll } from "@/src/shared/components/infinite-scroll/InfiniteScroll";
import { getMatchings } from "../actions/action";
import { ReadDto } from "@/src/shared/types/dtos/types";

export type MatchesProps = {
    user: AuthenticatedUser
    initialMatches: ReadDto<Matching, MatchingPermission>[];
    className?: string;
};
export function Matches({ user, initialMatches, className }: MatchesProps) {

    const [matches, setMatches] = useState<ReadDto<Matching, MatchingPermission>[]>(initialMatches);

    const page = matches.length > 0 ? 2 : 1;
    const pageSize = matches.length > 0 ? matches.length : 50;

    const items = matches.length > 0 ? matches.map(match => (
        <li key={match.data.id}>
            <MatchCompact dto={match} user={user} />
        </li>
    )) : [];

    async function loadMore(): Promise<boolean> {
        const response = await getMatchings(page, pageSize);
        const newMatches = response.value || [];
        setMatches(prev => [...prev, ...newMatches]);
        return newMatches.length > 0;
    }

    return (
        <div className={`flex flex-col gap-3 ${className}`}>
            {
                items.length > 0 ?
                    <InfiniteScroll className={`flex flex-col gap-3`} callback={loadMore}>
                        {items}
                    </InfiniteScroll> :
                    <span className={`text-(--primary-fc) text-(length:--primary-fs) mx-auto my-auto font-bold`}>
                        no matches yet
                    </span>
            }
        </div>
    );

}