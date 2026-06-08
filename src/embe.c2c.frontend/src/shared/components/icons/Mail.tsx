import { Mail as NextIconsMail } from "@deemlol/next-icons";
import { SVGAttributes } from "react";

export type MailProps = SVGAttributes<SVGElement> & {
    color?: string;
    size?: string | number;
    strokeWidth?: string | number;
    unread: boolean
    title?: string;
}

export default function Mail({ className, unread, title, ...rest }: MailProps) {
    return (
        <div className="relative" title={title}>
            <NextIconsMail  {...rest} />
            {unread && <span className="absolute top-0 -left-[2.5px] block h-2 w-2 rounded-full bg-(--color-secondary)"></span>}
        </div>
    )
}