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
        <div className={`w-full ${classNames} pb-4`}>
            <div className="w-full h-[1px] bg-(--surface-font-color) relative">
                <div className="absolute h-full bg-(--primary) transition-all duration-300" style={{ width: `${(progress - 1) / (steps.length - 1) * 100}%` }}>
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
                                backgroundColor: progress >= index + 1 ? "var(--primary)" : "var(--surface-font-color)"
                            }}
                            onClick={() => {
                                if (index + 1 < progress) {
                                    onClick?.(index);
                                }
                            }}
                        >
                            <span className="text-(length:--fs-secondary) absolute left-1/2 -translate-x-1/2 top-full">{step}</span>
                        </button>
                    ))
                }
            </div>
        </div>
    )
}