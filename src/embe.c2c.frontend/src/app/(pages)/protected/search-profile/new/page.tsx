import { getMe } from "@/src/features/auth/actions/action";
import SearchProfileForm from "@/src/features/search-profiles/components/SearchProfileForm";

export type NewSearchProfilePageProps = {

};
export default async function NewSearchProfilePage({ }: NewSearchProfilePageProps) {
  const getMeResponse = await getMe();
  if (!getMeResponse.success || !getMeResponse.value?.data) 
  {
    throw new Error("not implemented");
  }

  return (
    <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
      <h1>search-profile</h1>
      <SearchProfileForm user={getMeResponse.value.data} className="grow-1 overflow-y-scroll scrollbar-none" />
    </div>
  );
}