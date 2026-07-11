import Surface from "@/src/shared/components/surfaces/Surface";
import { ThumbsDown, ThumbsUp } from "lucide-react";
import { useCallback } from "react";

export type JudgeOverlayProps = {
    children: React.ReactNode;
    onJudge: (isPositive: boolean) => void;
    className?: string;
}
export default function JudgeOverlay({ children, onJudge, className }: JudgeOverlayProps) {

    const classNames = [className].filter(Boolean).join(" ");
    const judgePositive = useCallback(() => onJudge(true), [onJudge]);
    const judgeNegative = useCallback(() => onJudge(false), [onJudge]);

    return (

        <Surface className={`${classNames} relative`} variant="secondary" padding="none">
            {children}
            <button
                onClick={judgeNegative}
                className="
                    absolute 
                    bottom-10
                    left-10 
                    text-(--primary-fc) 
                    max-w-max 
                    max-h-max 
                    p-0 
                    rounded-full
                    bg-red-300
                    p-3
            ">
                <ThumbsDown className="
                    w-10
                    h-10
                "/>
            </button>
            <button
                onClick={judgePositive}
                className="
                    absolute 
                    bottom-10
                    right-10 
                    text-(--primary-fc) 
                    max-w-max 
                    max-h-max 
                    p-0 
                    rounded-full
                    bg-green-300
                    p-3
                ">
                <ThumbsUp className="
                    w-10
                    h-10
                "/>
            </button>
        </Surface>

    )

}