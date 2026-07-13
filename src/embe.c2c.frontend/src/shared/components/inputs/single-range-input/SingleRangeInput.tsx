import * as Slider from '@radix-ui/react-slider';
import Surface from '../../surfaces/Surface';
import ErrorMessage from '../ErrorMessage';
import Alert from '../../infos/Alert';

export type SingleRangeInputProps = {
    label: string;
    className?: string;
    min: number;
    max: number;
    step: number;
    value?: number;
    minStepsBetweenThumbs?: number;
    onChange: (value: number) => void;
    errorMessage?: string
    disabled?: boolean,
    disabledAlertChildren?: React.ReactNode
}

export default function SingleRangeInput({ disabled, disabledAlertChildren, errorMessage, label, className, min, max, step, value, minStepsBetweenThumbs, onChange }: SingleRangeInputProps) {

    const classNames = [
        className,
    ].filter(Boolean).join(" ");

    const sliderClassNames = [
        "absolute h-full rounded-full",
        disabled ? "bg-gray-300" : "bg-(--universal-primary-bg)"
    ].filter(Boolean).join(" ");

    const thumbClassNames = [
        "block h-5 w-5 rounded-full border-2 border-(--border-color) focus:outline-none",
        disabled ? "bg-gray-300" : "bg-(--primary-fc)"
    ].filter(Boolean).join(" ");

    return (
        <Surface className={`input-wrapper ${classNames} pb-9`} variant="inherit" padding="none">
            {
                <div className="flex flex-row items-center justify-center gap-1">
                    <span className="label mx-0">{label}</span>
                    {
                        disabled &&
                        <Alert>
                            {disabledAlertChildren}
                        </Alert>
                    }
                </div>
            }
            <Slider.Root
                disabled={disabled}
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
                    <Slider.Range className={sliderClassNames} />
                </Slider.Track>
                <Slider.Thumb className={thumbClassNames} >
                    <span className="absolute -bottom-6 right-0">{value ?? min}{value === max ? "+" : ""}</span>
                </Slider.Thumb>
            </Slider.Root>
            <ErrorMessage message={errorMessage} />
        </Surface>
    )

}