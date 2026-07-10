select c."Id" CandidateId, usp."Id" UserSearchProfileId, csp."Id" CandidateSearchProfileId
from "DomainUsers" u
inner join "SearchProfiles" usp on u."Id" = usp."UserId"
inner join "DomainUsers" c on (ST_Distance(u."Coordinates", c."Coordinates") <= usp."MaximumDistance" * 1000 or usp."MaximumDistance" is null)
inner join "SearchProfiles" csp on csp."UserId" = c."Id" and (ST_Distance(u."Coordinates", c."Coordinates") <= csp."MaximumDistance" * 1000 or csp."MaximumDistance" is null)
where 1=1
and u."Id" = '123' 
and c."Id" != u."Id"
and (
	exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = usp."Id" and spg."Gender" = c."Gender")
	or (select count(1) from "SearchProfileGender" spg where spg."SearchProfileId" = usp."Id") = 0
)
and (
	exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = csp."Id" and spg."Gender" = u."Gender")
	or (select count(1) from "SearchProfileGender" spg where spg."SearchProfileId" = csp."Id") = 0
)
and extract(year from age(CURRENT_DATE, u."BirthDate")) between coalesce(usp."AgeRangeMin", 0) and coalesce(usp."AgeRangeMax", 120)
and usp."Engagement_Frequency" = csp."Engagement_Frequency"
and usp."Engagement_Boundedness" = csp."Engagement_Boundedness"
and usp."Engagement_Medium" = csp."Engagement_Medium"
and (
	usp."Engagement_Boundedness" != 2 
	or daterange(usp."Engagement_StartDate", usp."Engagement_EndDate") && daterange(csp."Engagement_StartDate", csp."Engagement_EndDate")
)
and not exists (select * from "Blockings" b where b."BlockerUserId" = u."Id" and b."BlockedUserId" = c."Id")
and not exists (select * from "Blockings" b where b."BlockerUserId" = c."Id" and b."BlockedUserId" = u."Id")
and not exists (select * from "Matchings" m where m."UserId1" = u."Id" and m."UserId2" = c."Id")
and not exists (select * from "Matchings" m where m."UserId1" = c."Id" and m."UserId2" = u."Id")
and not exists (select * 
				from "Judgements" j 
				inner join "Candidates" can on can."Id" = j."CandidateId"
				where can."UserId" = u."Id" and can."CandidateUserId" = c."Id")
and not exists (select * 
				from "Judgements" j 
				inner join "Candidates" can on can."Id" = j."CandidateId"
				where can."UserId" = c."Id" and can."CandidateUserId" = u."Id" and j."IsPositive" = false)
and not exists (select * from "Candidates" can where can."UserId" = u."Id" and can."CandidateUserId" = c."Id" and can."UserSearchProfileId" = usp."Id" and can."CandidateSearchProfileId" = csp."Id")
offset 0 
limit 20