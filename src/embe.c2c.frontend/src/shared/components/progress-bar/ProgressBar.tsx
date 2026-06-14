import Surface from "../surfaces/Surface";

export type ProgressBarProps = {
    className?: string;
    steps: string[];
    progress: number;
    onClick?: (stepIndex: number) => void;
}

export default function ProgressBar({ className, steps, progress, onClick }: ProgressBarProps) {
    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`${classNames} w-full pb-6 pr-6`} variant="inherit">
            <div className="w-full h-[2px] bg-(--primary-fc) relative">
                <div className="absolute h-full bg-(--universal-primary-bg) transition-all duration-300" style={{ width: `${(progress - 1) / (steps.length - 1) * 100}%` }}>
                </div>
                {
                    steps.map((step, index) => (
                        <button
                            key={step}
                            className=
                            {
                                `
                                ${progress > index ? "cursor-pointer" : ""}
                                w-[10px] 
                                h-[10px] 
                                rounded-full
                                absolute top-1/2 
                                -translate-y-1/2`
                            }
                            style={{
                                left: `${(index) / (steps.length - 1) * 100}%`,
                                transform: `translate(-50%, 0)`,
                                backgroundColor: progress >= index + 1 ? "var(--universal-primary-bg)" : "var(--primary-fc)",
                            }}
                            onClick={() => {
                                if (index + 1 < progress) {
                                    onClick?.(index);
                                }
                            }}
                        >
                            <span className="text-(--primary-fc) text-(length:--secondary-fs) absolute left-1/2 -translate-x-1/2 top-full">{step}</span>
                        </button>
                    ))
                }
            </div>
        </Surface>
    )
}