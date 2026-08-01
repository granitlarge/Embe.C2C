import type { Metadata } from "next";
import { calSans } from "@/fonts";

import "./globals.css";
import Surface from "../shared/components/surfaces/Surface";
import NextTopLoader from "nextjs-toploader";

export const metadata: Metadata = {
  title: "rel.zone",
  description: "rel.zone",
};

export type RootLayoutProps = {
  children: React.ReactNode;
};

export default async function RootLayout({
  children,
}: RootLayoutProps) {

  return (

    <html
      lang="en"
    >

      <Surface as="body" className={`fs-group-primary flex flex-col h-dvh w-dvw px-2 pb-2 pt-1 ${calSans.variable} gap-0 max-w-[800px] mx-auto`} variant="primary" padding="none">

        <NextTopLoader color="white" showSpinner={false} />
        <main className="grow-1 overflow-y-scroll scrollbar-none flex flex-col overflow-x-hidden">
          {children}
        </main>
      </Surface>

    </html>

  );

}