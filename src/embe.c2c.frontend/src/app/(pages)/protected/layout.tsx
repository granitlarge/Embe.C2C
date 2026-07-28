import { SignalRProvider } from "@/src/shared/providers/signal-r";
import { ReactNode } from "react";

export type ProtectedLayoutProps = {
    children: ReactNode;
}
export default async function ProtectedLayout({ children }: ProtectedLayoutProps) {
    return (
        <SignalRProvider>
            <>
                {children}
            </>
        </SignalRProvider>

    )
}