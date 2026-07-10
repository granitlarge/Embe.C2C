import * as Slider from '@radix-ui/react-slider';
import Surface from '../../surfaces/Surface';

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
        <Surface className={`input-wrapper ${classNames} pb-9`} variant="inherit" padding="none">
            <span className="label">{label}</span>
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
                <Slider.Track className="relative h-2 w-full grow rounded-full bg-(--secondary-fc)">
                    <Slider.Range className="absolute h-full rounded-full bg-(--universal-primary-bg)" />
                </Slider.Track>
                <Slider.Thumb className="block h-5 w-5 rounded-full border-2 border-(--border-color) bg-(--primary-fc) focus:outline-none" >
                    <span className="absolute -bottom-6 right-0">{value ?? min}{value === max ? "+" : ""}</span>
                </Slider.Thumb>
            </Slider.Root>
        </Surface>
    )
}