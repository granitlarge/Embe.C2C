import { hasUnread } from "@/src/features/notifications/actions/action"

export default async function SwipePage() {
    const result = await hasUnread();
    return (
        <>
        </>
    )
}