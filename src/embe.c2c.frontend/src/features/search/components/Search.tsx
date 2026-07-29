"use client";

import { useCallback, useState } from "react";
import JudgeOverlay from "./JudgeOverlay";
import * as api from "../actions/action";
import Profile from "@/src/shared/components/user/Profile";
import MainNav from "@/src/shared/components/nav/MainNav";
import { SlidersHorizontal } from "lucide-react";
import Link from "@/src/shared/components/Links/Link";
import { useRouter } from "nextjs-toploader/app";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { Candidate, CandidatePermission, NotificationType, PositivelyJudgedNotification, SearchProfile, SearchProfilePermission } from "@/src/shared/types/domain/aggregates";
import Button from "@/src/shared/components/buttons/Button";
import { Routes } from "@/src/shared/routes";
import { LocalStore } from "@/src/shared/local-store";
import { useApplicationStore } from "@/src/shared/stores/provider";

type HeaderProps = {
    hasSearchProfiles: boolean;
}
function Header({ hasSearchProfiles }: HeaderProps) {

    const router = useRouter();

    return (
        <div>
            <header className="flex flex-row items-center">
                <h1 className="truncate">search</h1>
                {
                    hasSearchProfiles &&
                    <Button className="ml-auto max-w-max" onClick={() => router.push(Routes.protected.searchProfiles)} intent="none">
                        <SlidersHorizontal className="w-6 h-6" />
                    </Button>
                }
            </header>
        </div>
    )

}

export type SearchProps = {
    searchProfiles: ReadDto<SearchProfile, SearchProfilePermission>[];
    hasSearchProfiles: boolean;
    candidates: ReadDto<Candidate, CandidatePermission>[];
    className?: string;
}
export default function Search({ searchProfiles, candidates: initialCandidates, className, hasSearchProfiles }: SearchProps) {

    const notifications = useApplicationStore(s => s.notifications);
    const setNotifications = useApplicationStore(s => s.setNotifications);

    const router = useRouter();
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

       const candidate = candidates[0];
       setCandidates(prev => prev.slice(1));

       try {

           const response = await api.judge(candidates[0].data.id, isPositive);
           if (!response.success) {

               setCandidates(prev => [...prev, candidate]);

           } else {

               router.refresh();

               if (response.value !== undefined) {

                   const matching = response.value;

                   setNotifications(prev =>
                       prev.filter(n => {
                           if (n.data.type !== NotificationType.PositivelyJudged) {
                               return true;
                           }
                           const asPositivelyJudged = n.data as PositivelyJudgedNotification;
                           return asPositivelyJudged.userId !== matching?.data.userId1 && asPositivelyJudged.userId !== matching?.data.userId2;
                       })
                   )

               }

               if (candidates.length === 0) {

                   try {

                       await loadCandidates();

                   } catch (e) {

                   }

               }

           }

       } catch (e) {

           setCandidates(prev => [...prev, candidate]);

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
                            <Profile className="grow-1" userSearchProfile={searchProfiles.find(sp => sp.data.id === candidates[0].data.userSearchProfileId)?.data} candidate={candidates[0].data.candidate?.data!} candidateSearchProfile={candidates[0].data.candidateSearchProfile.data} />
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
                    <Link className="text-(length:--primary-fs) font-bold" href={Routes.protected.createSearchProfile}>creating a search profile</Link>
                </div>
            }
            <MainNav className="grow-0" />
        </div>
    )

}