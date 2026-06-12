import * as Slider from '@radix-ui/react-slider';

export type SingleRangeInputProps = {
    label: string;
    className?: string;
    min: number;
    max: number;
    step: number;
    value?: number;
    minStepsBetweenThumbs?: number;
    onChange: (value: number) => void;
}

export default function SingleRangeInput({ label, className, min, max, step, value, minStepsBetweenThumbs, onChange }: SingleRangeInputProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <label className="label w-full">
            <span className="label-text">{label}</span>
            <Slider.Root
                value={[value ?? min]}
                min={min}
                max={max}
                step={step}
                onValueChange={(newValue) => {
                    onChange(newValue[0]);
                }}
                className={`relative flex w-full touch-none select-none items-center ${classNames}`}
            >
                <Slider.Track className="relative h-2 w-full grow rounded-full bg-(--background)">
                    <Slider.Range className="absolute h-full rounded-full bg-(--primary)" />
                </Slider.Track>
                <Slider.Thumb className="block h-5 w-5 rounded-full border-2 border-(--primary) bg-white focus:outline-none" >
                    <span className="absolute -bottom-8 text-(length:--fs-secondary)">{value ?? min}</span>
                </Slider.Thumb>
            </Slider.Root>
        </label>
    )
}