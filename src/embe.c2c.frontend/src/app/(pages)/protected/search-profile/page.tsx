import SearchProfileForm from "@/src/features/search-profiles/components/SearchProfileForm";
import MainNav from "@/src/shared/components/nav/MainNav";

export type NewSearchProfilePageProps = {

};
export default async function NewSearchProfilePage({}: NewSearchProfilePageProps) {
  return (
    <div className="flex flex-col grow-1 gap-3 overflow-y-scroll scrollbar-none">
      <h1>search-profile</h1>
      <SearchProfileForm className="grow-1 overflow-y-scroll scrollbar-none" />
      <MainNav />
    </div>
  );
}