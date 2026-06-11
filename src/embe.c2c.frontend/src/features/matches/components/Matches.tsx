"use client";

import { AuthenticatedUser } from "@/src/shared/user";
import { MatchCompact } from "./MatchCompact";
import { Matching } from "@/src/shared/types/domain/aggregates";
import { useState } from "react";
import { InfiniteScroll } from "@/src/shared/components/infinite-scroll/InfiniteScroll";
import { getMatches } from "../actions/action";

export type MatchesProps = {
    user: AuthenticatedUser
    className?: string;
    initialMatches: Matching[];
};

export function Matches({ className, user, initialMatches }: MatchesProps) {

    const [matches, setMatches] = useState<Matching[]>(initialMatches);
    const classNames = [className].filter(Boolean).join(" ");

    const page = matches.length > 0 ? 2 : 1;
    const pageSize = matches.length > 0 ? matches.length : 50;

    const items = matches.length > 0 ? matches.map(match => (
        <li key={match.id}>
            <MatchCompact match={match} user={user} />
        </li>
    )) : [];

    async function loadMore(): Promise<boolean> {
        const response = await getMatches(page, pageSize);
        const newMatches = response.value || [];
        setMatches(prev => [...prev, ...newMatches]);
        return newMatches.length > 0;
    }

    return (
        <div>
            {
                items.length > 0 ?
                    <InfiniteScroll className="flex flex-col gap-3" callback={loadMore}>
                        {items}
                    </InfiniteScroll> :
                    <span className="text-(length:--fs-5) mx-auto my-auto">
                        no matches yet
                    </span>
            }
        </div>
    );

}