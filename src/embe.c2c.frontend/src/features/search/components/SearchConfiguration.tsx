"use client";

import { InfiniteScroll } from "@/src/shared/components/scroll/infinite-scroll/InfiniteScroll";
import { SearchProfile, SearchProfilePermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { useEffect, useState } from "react";
import SearchProfileCompact from "./SearchProfileCompact";
import { getAllSearchProfiles } from "../actions/action";

export type SearchConfigurationProps = {
    className?: string;
}
export default function SearchConfiguration({ className }: SearchConfigurationProps) {

    const [searchProfiles, setSearchProfiles] = useState<ReadDto<SearchProfile, SearchProfilePermission>[]>([]);
    const page = searchProfiles.length > 0 ? 2 : 1;
    const pageSize = searchProfiles.length > 0 ? searchProfiles.length : 10;

    useEffect(() => {
        async function loadInitialSearchProfiles() {
            const response = await getAllSearchProfiles(page, pageSize);
            if (!response.success) {
                throw new Error("not implemented");
            }
            setSearchProfiles(response.value ?? []);
        }
        loadInitialSearchProfiles();
    }, [])

    async function loadSearchProfiles(): Promise<boolean> {
        const response = await getAllSearchProfiles(page, pageSize);
        if (!response.success) {
            throw new Error("not implemented");
        }
        setSearchProfiles(prev => [...prev, ...response.value ?? []]);
        return response.value!.length === pageSize;
    }

    const classNames = [className].filter(Boolean).join(" ");
    return (
        <InfiniteScroll className={`${classNames} flex flex-col gap-2 w-full`} callback={loadSearchProfiles}>
            {
                searchProfiles.map(searchProfile => (
                    <SearchProfileCompact key={searchProfile.data.id} searchProfile={searchProfile} />
                ))
            }
        </InfiniteScroll>
    )

}