"use client";

import { useCallback, useState } from "react";
import JudgeOverlay from "./JudgeOverlay";
import * as api from "../actions/action";
import Profile from "@/src/shared/components/user/Profile";
import MainNav from "@/src/shared/components/nav/MainNav";
import Link from "next/link";
import { SlidersHorizontal } from "lucide-react";
import Modal from "@/src/shared/components/modal/Modal";
import SearchProfiles from "./SearchProfiles";
import { GeneratedCandidate } from "../actions/type";

type HeaderProps = {
    hasSearchProfiles: boolean;
}
function Header({ hasSearchProfiles }: HeaderProps) {

    const [isSearchConfigurationOpen, setIsSearchConfigurationOpen] = useState(false);

    return (
        <div>
            <header className="flex flex-row items-center">
                <h1 className="truncate">search</h1>
                {
                    hasSearchProfiles &&
                    <button className="ml-auto" onClick={() => setIsSearchConfigurationOpen(true)}>
                        <SlidersHorizontal className="w-6 h-6" />
                    </button>
                }
            </header>
            <Modal
                className="surface-secondary p-3 gap-3"
                closed={() => setIsSearchConfigurationOpen(false)}
                hidden={!isSearchConfigurationOpen}
                header="search profiles"
            >
                <SearchProfiles />
            </Modal>
        </div>
    )

} 

export type SearchProps = {
    hasSearchProfiles: boolean;
    candidates: GeneratedCandidate[];
    className?: string;
}
export default function Search({ candidates: initialCandidates, className, hasSearchProfiles }: SearchProps) {

    const classNames = [className].filter(Boolean).join(" ");

    const [candidates, setCandidates] = useState(initialCandidates);
    const judgeCallback = useCallback(judge, [candidates[0]]);

    async function loadCandidates() {
        const response = await api.generateCandidates();
        if (!response.success) {
            throw new Error("Not implemented");
        } else {
            setCandidates(candidates => [...candidates, ...response.value!]);
        }
    }

    async function judge(isPositive: boolean) {
        const response = await api.judge(candidates[0].id, isPositive);
        if (!response.success) {
            throw new Error("Not implemented");
        } else {
            if (candidates.length === 0) {
                await loadCandidates();
            } else {
                setCandidates(prev => prev.slice(1));
            }
        }
    }

    return (
        <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
            <Header hasSearchProfiles={hasSearchProfiles} />
            {
                hasSearchProfiles &&
                <>
                    {
                        candidates[0] &&
                        <JudgeOverlay className={`${classNames} flex flex-col`} onJudge={judgeCallback}>
                                <Profile className="grow-1" candidate={candidates[0].candidate.data} candidateSearchProfile={candidates[0].candidateSearchProfile.data} />
                        </JudgeOverlay>
                    } {
                        !candidates[0] &&
                        <div className={`${classNames} flex flex-col items-center justify-center`}>
                            <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">no more candidates</span>
                        </div>
                    }
                </>
            }
            {
                !hasSearchProfiles &&
                <div className="grow-1 flex flex-col justify-center items-center">
                    <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">find people that match your vibe by</span>
                    <Link className="text-(length:--primary-fs) font-bold" href="/protected/search-profile">creating a search profile</Link>
                </div>
            }
            <MainNav className="grow-0" />
        </div>
    )

}