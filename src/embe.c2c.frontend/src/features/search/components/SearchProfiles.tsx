"use client";

import { InfiniteScroll } from "@/src/shared/components/scroll/infinite-scroll/InfiniteScroll";
import { SearchProfile, SearchProfilePermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { useState } from "react";
import SearchProfileCompact from "./SearchProfileCompact";
import { getAllSearchProfiles } from "../actions/action";
import Surface from "@/src/shared/components/surfaces/Surface";
import Button from "@/src/shared/components/buttons/Button";
import { Plus } from "lucide-react";
import { useRouter } from "next/navigation";

export type SearchProfilesProps = {
    className?: string;
    searchProfiles?: ReadDto<SearchProfile, SearchProfilePermission>[];
}
export default function SearchProfiles({ className, searchProfiles: initialSearchProfiles }: SearchProfilesProps) {

    const router = useRouter();

    const [searchProfiles, setSearchProfiles] = useState<ReadDto<SearchProfile, SearchProfilePermission>[]>(initialSearchProfiles ?? []);
    const page = searchProfiles.length > 0 ? 2 : 1;
    const pageSize = searchProfiles.length > 0 ? searchProfiles.length : 10;

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
        <Surface className={`${classNames} flex flex-col gap-2 w-full`} variant="inherit">
            <div className="flex flex-row gap-1 items-center">
                <h1>search profiles</h1>
                <Button
                    className="max-w-max max-h-max ml-auto flex flex-row gap-1 items-center justify-between"
                    intent="create"
                    onClick={() => { router.push("/protected/search-profile/new") }}
                >
                    <Plus className="w-(--primary-fs) h-(--primary-fs)" />
                    <span>create</span>
                </Button>
            </div>
            <InfiniteScroll className={`flex flex-col gap-2 w-full grow-1`} callback={loadSearchProfiles}>
                {
                    searchProfiles.map(searchProfile => (
                        <SearchProfileCompact key={searchProfile.data.id} searchProfile={searchProfile} />
                    ))
                }
            </InfiniteScroll>
        </Surface>
    )

}