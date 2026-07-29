import * as Slider from '@radix-ui/react-slider';
import Surface from '../../surfaces/Surface';
import ErrorMessage from '../ErrorMessage';
import InfoModal, { InfoType } from '../../infos/InfoModal';

export type DualRangeInputProps = {
    label: string;
    className?: string;
    min: number;
    max: number;
    step: number;
    value?: [number, number];
    minStepsBetweenThumbs?: number;
    onChange: (value: [number, number]) => void;
    errorMessage?: string;
    info?: string;
    infoType?: InfoType
}

export default function DualRangeInput({ info, infoType, errorMessage, label, className, min, max, step, value, minStepsBetweenThumbs, onChange }: DualRangeInputProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${classNames} pb-9`} variant="inherit" padding="none">
            <div className="flex gap-1 justify-center items-center">
                <span className="label m-0">{label}</span>
                {info && infoType && <InfoModal info={info} type={infoType} />}
            </div>
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
                <Slider.Track className="relative h-2 w-full grow rounded-full bg-(--secondary-fc)">
                    <Slider.Range className="absolute h-full rounded-full bg-(--universal-primary-bg)" />
                </Slider.Track>
                <Slider.Thumb className="block h-5 w-5 rounded-full border-2 border-(--border-color) bg-(--primary-fc) focus:outline-none" >
                    <span className="absolute -bottom-6 left-0 text-(--primary-fc)">{value ? value[0] : min}</span>
                </Slider.Thumb>
                <Slider.Thumb className="block h-5 w-5 rounded-full border-2 border-(--border-color) bg-(--primary-fc) focus:outline-none" >
                    <span className="absolute -bottom-6 right-0 text-(--primary-fc)">{value ? value[1] : max}{value && value[1] == max ? "+" : ""}</span>
                </Slider.Thumb>
            </Slider.Root>
            <ErrorMessage message={errorMessage} />
        </Surface>
    )

}