import { hasUnread } from "@/src/features/notifications/actions/action"

export default async function DatingPage() {
    const result = await hasUnread();
    return (
        <>
        </>
    )
}