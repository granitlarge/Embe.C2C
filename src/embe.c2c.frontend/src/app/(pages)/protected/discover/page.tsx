import MainNav from "@/src/shared/components/nav/MainNav"

export type DiscoverPageProps = {

}
export default async function DiscoverPage({ }: DiscoverPageProps) {
    return (
        <div className="flex flex-col grow-1 gap-3">
            <h1>discover</h1>
            <div className="grow-1">
            </div>
            <MainNav className="grow-0" />
        </div>
    )
}