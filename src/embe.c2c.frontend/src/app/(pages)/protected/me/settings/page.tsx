import BackButton from "@/src/shared/components/buttons/BackButton"

export type SettingsPageProps = {

}
export default async function SettingsPage({ }: SettingsPageProps) {
    return (
        <div className="flex flex-col gap-3">
            <div className="flex justify-between items-center">
                <BackButton />
                <h1>settings</h1>
            </div>
        </div>
    )
}