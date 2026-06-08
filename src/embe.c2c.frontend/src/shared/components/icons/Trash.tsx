import { Trash2 } from "@deemlol/next-icons";
import { SVGAttributes } from "react";

export interface TrashProps extends SVGAttributes<SVGElement> {
    color?: string;
    size?: string | number;
    strokeWidth?: string | number;
    title?: string
}

export default function Trash({ title, ...props }: TrashProps) {
    return (
        <div title={title}>
            <Trash2 {...props} />
        </div>
    )
}