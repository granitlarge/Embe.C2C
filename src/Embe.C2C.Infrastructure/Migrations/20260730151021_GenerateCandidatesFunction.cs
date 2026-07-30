using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GenerateCandidatesFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                create or replace function generate_candidates(userId uuid)
                returns int
                language plpgsql
                as $$
                begin

                    create temp table "temp_candidates" (like "Candidates") on commit drop;

                    insert into "temp_candidates"
                    select 

                        uuidv7(),
                        userId "UserId", 
                        c."Id" "CandidateId", 
                        usp."Id" "UserSearchProfileId", 
                        csp."Id" "CandidateSearchProfileId",
                        now() at time zone 'utc',
                        null

                    from "DomainUsers" u

                    inner join "SearchProfiles" usp on u."Id" = usp."UserId"
                    inner join "DomainUsers" c on (ST_Distance(u."Coordinates", c."Coordinates") <= usp."MaximumDistance" * 1000 or usp."MaximumDistance" is null)
                    inner join "SearchProfiles" csp on csp."UserId" = c."Id" and (ST_Distance(u."Coordinates", c."Coordinates") <= csp."MaximumDistance" * 1001 or csp."MaximumDistance" is null)
                    inner join "SearchProfileEmbeddings" uspe on usp."Id" = uspe."SearchProfileId"
                    inner join "SearchProfileEmbeddings" cspe on csp."Id" = cspe."SearchProfileId"

                    where 1=1
                    and u."Id" = userId
                    and c."Id" != u."Id"
                    and usp."Active" = true
                    and csp."Active" = true
                    and usp."RelationshipType" = csp."RelationshipType"
                    and exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = usp."Id" and spg."Gender" = c."Gender")
                    and exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = csp."Id" and spg."Gender" = u."Gender")
                    and extract(year from age(CURRENT_DATE, u."BirthDate")) between coalesce(csp."AgeRangeMin", 18) and coalesce(csp."AgeRangeMax", 120)
                    and extract(year from age(CURRENT_DATE, c."BirthDate")) between coalesce(usp."AgeRangeMin", 18) and coalesce(usp."AgeRangeMax", 120)
                    and (
                        -- we're adding some fuzziness to the search
                        -- if the desired frequency differs from "once" accept a mismatch of 1 step, so those who seek daily will be able to see those who seek weekly,
                        -- those who seek weekly will be able to see those who seek monthly, and vice versa.
                        (usp."Engagement_Frequency" = 0 or csp."Engagement_Frequency" = 0) and usp."Engagement_Frequency" = csp."Engagement_Frequency"
                        or abs(usp."Engagement_Frequency" - csp."Engagement_Frequency") <= 1
                    )
                    and usp."Engagement_Boundedness" = csp."Engagement_Boundedness"
                    and (
                        -- fuzzying here as well
                        -- if someone is searching for "hybrid" and someone else is searching for "virtual", we'll show em to each other despite there
                        -- not being a perfect match, one of them could compromise 	
                        usp."Engagement_Medium" = csp."Engagement_Medium"
                        or usp."Engagement_Medium" = 2
                        or csp."Engagement_Medium" = 2
                    )
                    and (
                        usp."Engagement_Boundedness" != 2 
                        or daterange(usp."Engagement_StartDate", usp."Engagement_EndDate") && daterange(csp."Engagement_StartDate", csp."Engagement_EndDate")
                    )
                    and not exists (select * from "Blockings" b where b."BlockerUserId" = u."Id" and b."BlockedUserId" = c."Id")
                    and not exists (select * from "Blockings" b where b."BlockerUserId" = c."Id" and b."BlockedUserId" = u."Id")
                    and not exists (select * from "Matchings" m 
                                    where m."UserId1" = u."Id" and m."UserId2" = c."Id" and
                                    m."UserId1SearchProfileId" = usp."Id" and
                                    m."UserId2SearchProfileId" = csp."Id")
                    and not exists (select * from "Matchings" m 
                                    where m."UserId1" = c."Id" and m."UserId2" = u."Id" and
                                    m."UserId1SearchProfileId" = csp."Id" and 
                                    m."UserId2SearchProfileId" = usp."Id")
                    and not exists (select * 
                                    from "Candidates" can
                                    where can."UserId" = c."Id" and can."CandidateUserId" = u."Id" and can."Judgement" = false and
                                    can."UserSearchProfileId" = csp."Id" and
                                    can."CandidateSearchProfileId" = usp."Id")
                    and not exists (select * 
                                    from "Candidates" can 
                                    where can."UserId" = u."Id" 
                                    and can."CandidateUserId" = c."Id"
                                    and can."UserSearchProfileId" = usp."Id" 
                                    and can."CandidateSearchProfileId" = csp."Id")

                    order by uspe."Embedding" <=> cspe."Embedding"
                    offset 0 
                    limit 20;

                    insert into "Candidates"
                    select uuidv7(), tc."CandidateUserId", tc."UserId", tc."CandidateSearchProfileId", tc."UserSearchProfileId", now() at time zone 'utc', null
                    from "temp_candidates" tc
                    union all
                    select * from "temp_candidates";


                    return (select count(1) from "temp_candidates");

                end;
                $$
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop function if exists generate_candidates(uuid)");
        }
    }
}
