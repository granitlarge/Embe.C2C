import { Read } from "@/src/shared/apis/api";
import { NullGuid } from "@/src/shared/cache";
import { Judgement, JudgementPermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { getAuthenticatedUser } from "@/src/shared/user";

export async function getPositiveJudgements(page: number, size: number) {

    const user = await getAuthenticatedUser();
    const response = Read<ReadDto<Judgement, JudgementPermission>[]>
        (
            `${process.env.API_URL}/api/judgement/positive?page=${page}&size=${size}`,
            {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                },
                next: {
                    tags: [
                        `user:${user?.userId ?? NullGuid}:judgement`
                    ],
                }
            }
        );

    return response;

}