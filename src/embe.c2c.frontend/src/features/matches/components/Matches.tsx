"use client";

import { AuthenticatedUser } from "@/src/shared/user";
import { MatchCompact } from "./MatchCompact";
import { InfiniteScroll } from "@/src/shared/components/scroll/infinite-scroll/InfiniteScroll";
import { getMatchings } from "../actions/action";
import { useApplicationStore } from "@/src/shared/stores/provider";

export type MatchesProps = {
    user: AuthenticatedUser
    className?: string;
};
export function Matches({ user, className }: MatchesProps) {

    const matches = useApplicationStore(s => s.matchings);
    const setMatches = useApplicationStore(s => s.setMatchings);

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