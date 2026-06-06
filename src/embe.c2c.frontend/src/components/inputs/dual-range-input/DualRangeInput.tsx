import * as Slider from '@radix-ui/react-slider';

export type DualRangeInputProps = {
    label: string;
    className?: string;
    min: number;
    max: number;
    step: number;
    value?: [number, number];
    minStepsBetweenThumbs?: number;
    onChange: (value: [number, number]) => void;
}

export default function DualRangeInput({ label, className, min, max, step, value, minStepsBetweenThumbs, onChange }: DualRangeInputProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <label className="label w-full">
            <span className="label-text">{label}</span>
            <Slider.Root
                value={value ?? [min, max]}
                min={min}
                max={max}
                step={step}
                minStepsBetweenThumbs={minStepsBetweenThumbs}
                onValueChange={(newValue) => {
                    onChange(newValue as [number, number]);
                }}
                className={`relative flex w-full touch-none select-none items-center ${classNames}`}
            >
                <Slider.Track className="relative h-2 w-full grow rounded-full bg-(--color-primary)">
                    <Slider.Range className="absolute h-full rounded-full bg-(--color-secondary)" />
                </Slider.Track>
                <Slider.Thumb className="block h-5 w-5 rounded-full border-2 border-(--color-primary) bg-white focus:outline-none" >
                    <span className="absolute -bottom-8 text-md">{value ? value[0] : min}</span>
                </Slider.Thumb>
                <Slider.Thumb className="block h-5 w-5 rounded-full border-2 border-(--color-primary) bg-white focus:outline-none" >
                    <span className="absolute -bottom-8 text-md">{value ? value[1] : max}</span>
                </Slider.Thumb>
            </Slider.Root>
        </label>
    )

}