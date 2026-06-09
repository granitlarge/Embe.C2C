export type ProgressBarProps = {
    className?: string;
    steps: number;
    progress: number;
}

export default function ProgressBar({ className, steps, progress }: ProgressBarProps)
{
    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <div className={`w-full ${classNames}`}>
            <div className="w-full h-[1px] bg-(--surface-font-color) relative">
                <div className="absolute h-full bg-(--primary) transition-all duration-300" style={{ width: `${(progress - 1) / (steps - 1) * 100}%` }}>
                </div>
                {
                    [...Array(steps)].map((_, index) => (
                        <div
                            key={index}
                            className=
                            {
                                `w-[5px] 
                                h-[5px] 
                                rounded-full
                                absolute top-1/2 
                                -translate-y-1/2`
                            }
                            style={{
                                left: `${(index) / (steps - 1) * 100}%`,
                                backgroundColor: progress >= index + 1 ? "var(--primary)" : "var(--surface-font-color)"
                            }}
                        />
                    ))
                }
            </div>
        </div>
    )
}