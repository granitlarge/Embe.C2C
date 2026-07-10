"use client";

import { User as UserTypeDef, UserPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { useCallback, useState } from "react";
import JudgeOverlay from "./JudgeOverlay";
import * as api from "../actions/action";
import Profile from "@/src/shared/components/user/Profile";

export type FindProps = {
    candidates: ReadDto<UserTypeDef, UserPermission>[];
    className?: string;
}

export default function Find({ candidates: initialCandidates, className }: FindProps) {

    const classNames = [className].filter(Boolean).join(" ");

    const [candidates, setCandidates] = useState(initialCandidates);
    const judgeCallback = useCallback(judge, [candidates[0]]);

    async function loadCandidates() {
        const response = await api.getCandidates();
        if (!response.success) {
            throw new Error("Not implemented");
        } else {
            setCandidates(candidates => [...candidates, ...response.value!]);
        }
    }

    async function judge(isPositive: boolean) {
        const response = await api.judge(candidates[0].data.id, isPositive);
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
        <>

            {
                candidates[0] &&
                <JudgeOverlay className={`${classNames} flex flex-col`} onJudge={judgeCallback}>
                    <Profile className="grow-1" user={candidates[0].data} />
                </JudgeOverlay>
            } {
                !candidates[0] &&
                <div className={`${classNames} flex flex-col items-center justify-center`}>
                    <span className="text-(--primary-fc) text-(length:--primary-fs) font-bold">no more candidates</span>
                </div>
            }
        </>
    )

}