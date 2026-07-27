"use client";

import { ChevronLeft } from "lucide-react"
import Button from "./Button"
import { useRouter } from "nextjs-toploader/app"

export type BackButtonProps = {

}

export default function BackButton({ }: BackButtonProps) {
    const router = useRouter();
    return (
        <Button className="max-w-max" intent="navigate" onClick={router.back}>
            <ChevronLeft className="w-(--primary-fs) h-(--primary-fs)" />
        </Button>
    )
}