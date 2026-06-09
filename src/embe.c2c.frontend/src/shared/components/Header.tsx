import NotificationsMenu from "@/src/features/notifications/components/notifications-menu/NotificationsMenu";
import { getAuthenticatedUser } from "../user";

export default async function Header() {

    const user = await getAuthenticatedUser();

    return (
        <header className="flex items-center justify-between bg-gray-800 text-white p-4 rounded-lg">
            {user && <NotificationsMenu className="ml-auto" />}
        </header>
    );

}