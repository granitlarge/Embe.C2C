"use client";

import Button from "@/src/components/buttons/Button";
import { EmailInput } from "@/src/components/inputs/email-input/EmailInput";
import { useState } from "react";
import DateInput from "@/src/components/inputs/date-input/DateInput";
import SelectInput from "@/src/components/inputs/select-input/SelectInput";
import DualRangeInput from "@/src/components/inputs/dual-range-input/DualRangeInput";
import ProgressBar from "@/src/components/progress-bar/ProgressBar";
import ImageGallery from "@/src/components/inputs/image/gallery/ImageGallery";
import * as z from "zod";
import { checkAccountExists as checkAccountExists } from "../apis/AccountExists";
import { useRouter } from "next/navigation";
import TextInput from "@/src/components/inputs/text-input/TextInput";
import { register } from "@/src/features/auth/apis/Register";
import { Gender } from "@/src/shared/types/value-objects";

export type RegisterFormProps = {
    className?: string;
}

type Step =
    "email" |
    "account exists" |
    "password" |
    "profile" |
    "dating preferences" |
    "images" |
    "success";

type EmailStepProps = {
    navigate: (step: Step) => void;
    setEmail: (email: string) => void;
    value?: string
}
function EmailStep({ navigate, setEmail, value }: EmailStepProps) {

    const emailSchema = z.email({ message: "please enter a valid email" });
    const [email, setEmailState] = useState<string | undefined>(value);
    const [emailError, setEmailError] = useState<string | undefined>(undefined);

    async function onNavigate() {
        const result = await emailSchema.safeParseAsync(email);
        if (!result.success) {
            setEmailError(result.error.issues[0].message);
            return;
        } else {
            const accountExists = await checkAccountExists(email!);
            if (accountExists) {
                navigate("account exists")
            } else {
                setEmailError(undefined);
                setEmail(result.data);
                navigate("password");
            }
        }
    }

    return (
        <div className="flex flex-col gap-3">
            <EmailInput value={email} onChange={setEmailState} valid={emailError === undefined} errorMessage={emailError} />
            <Button className="max-w-xs" onClick={() => onNavigate()}>next</Button>
        </div>
    )

}

function AccountExistsStep() {
    const router = useRouter();
    function onClick() {
        router.push("/login");
    }
    return (
        <Button onClick={onClick}>login</Button>
    )
}

type PasswordStepProps = {
    navigate: (step: Step) => void;
    setPassword: (password: string) => void;
}
function PasswordStep({ navigate, setPassword }: PasswordStepProps) {

    const validationSchema = z.object({
        password: z.string(),
        confirmPassword: z.string()
    }).refine((data) => data.password === data.confirmPassword, {
        message: "passwords do not match",
    });

    const [password, setPasswordState] = useState<string | undefined>(undefined);
    const [confirmPassword, setConfirmPasswordState] = useState<string | undefined>(undefined);
    const [error, setError] = useState<string | undefined>(undefined);

    function next() {
        const result = validationSchema.safeParse({ password, confirmPassword });
        if (!result.success) {
            setError(result.error.issues[0].message);
            return;
        } else {
            setError(undefined);
            setPassword(password!);
            navigate("profile");
        }
    }

    return (
        <div className="flex flex-col gap-3">
            <TextInput label="password" type="password" value={password} onChange={setPasswordState} valid={error === undefined} errorMessage={undefined} />
            <TextInput label="confirm password" type="password" value={confirmPassword} onChange={setConfirmPasswordState} valid={error === undefined} errorMessage={error} />
            <Button className="max-w-xs" onClick={next}>next</Button>
        </div>
    )

}

type ProfileStepProps = {
    navigate: (step: Step) => void;
    setGender: (gender: Gender) => void;
    setBirthDate: (birthDate: string) => void;
}
function ProfileStep({ navigate, setGender, setBirthDate }: ProfileStepProps) {

    const year = new Date().getFullYear();
    const month = new Date().getMonth() + 1;
    const day = new Date().getDate();

    const minDate = `${year - 120}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;
    const maxDate = `${year - 18}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;
    const [birthDate, setBirthDateState] = useState(maxDate);

    const genders = [
        { value: Gender.Male.toString(), label: "male" },
        { value: Gender.Female.toString(), label: "female" },
        { value: Gender.TransMale.toString(), label: "trans male" },
        { value: Gender.TransFemale.toString(), label: "trans female" },
        { value: Gender.Other.toString(), label: "other" },
    ];
    const [gender, setGenderState] = useState(genders[0].value);

    function onNext() {
        setGender
            (
                gender === "male" ? Gender.Male :
                    gender === "female" ? Gender.Female :
                        gender === "trans male" ? Gender.TransMale :
                            gender === "trans female" ? Gender.TransFemale :
                                Gender.Other
            );
        setBirthDate(birthDate);
        navigate("dating preferences");
    }

    return (
        <div className="flex flex-col gap-3 items-center w-full">
            <DateInput
                label={"date of birth"}
                minDate={minDate}
                maxDate={maxDate}
                value={birthDate}
                onChange={setBirthDateState}
            />
            <SelectInput className="w-full" label="gender" options={genders} value={[gender]} onChange={(genders) => setGenderState(genders[0])} />
            <Button className="max-w-xs" onClick={onNext}>next</Button>
        </div>
    )
}

type PreferencesStepProps = {
    onGendersChange?: (genders: Gender[]) => void;
    onAgeRangeChange?: (ageRange: { lower: number, higher: number }) => void;
    onDistanceRangeChange?: (distanceRange: { lower: number, higher: number }) => void;
    navigate: (step: Step) => void;
}
function PreferencesStep({ onGendersChange, onAgeRangeChange, onDistanceRangeChange, navigate }: PreferencesStepProps) {

    const validationSchema = z.object({
        preferredGenders: z.array(z.nativeEnum(Gender)).min(1, { message: "please select at least one gender" }),
    });

    const [gendersError, setGendersError] = useState<string | undefined>(undefined);

    const genders = [
        { value: Gender.Male.toString(), label: "male" },
        { value: Gender.Female.toString(), label: "female" },
        { value: Gender.TransMale.toString(), label: "trans male" },
        { value: Gender.TransFemale.toString(), label: "trans female" },
        { value: Gender.Other.toString(), label: "other" },
    ];

    const minAgeRange = 18;
    const maxAgeRange = 100;
    const [selectedGenders, setSelectedGenders] = useState<string[]>([]);
    const [ageRange, setAgeRange] = useState<{ lower: number, higher: number }>({ lower: minAgeRange, higher: maxAgeRange });
    const minDistanceRange = 0;
    const maxDistanceRange = 160;
    const [distanceRange, setDistanceRange] = useState<{ lower: number, higher: number }>({ lower: minDistanceRange, higher: maxDistanceRange });

    function next() {

        const result = validationSchema.safeParse({
            preferredGenders: selectedGenders.map(g => g === "male" ? Gender.Male :
                g === "female" ? Gender.Female :
                    g === "trans male" ? Gender.TransMale :
                        g === "trans female" ? Gender.TransFemale :
                            Gender.Other
            )
        });

        if (!result.success) {
            setGendersError(result.error.issues[0].message);
            return;
        }

        onGendersChange?.(
            selectedGenders.map(g => g === "male" ? Gender.Male :
                g === "female" ? Gender.Female :
                    g === "trans male" ? Gender.TransMale :
                        g === "trans female" ? Gender.TransFemale :
                            Gender.Other
            )
        );
        onAgeRangeChange?.(ageRange);
        onDistanceRangeChange?.(distanceRange);
        navigate("images");
    }

    return (
        <div className="flex flex-col gap-3 w-full items-center">
            <SelectInput
                options={genders}
                multiple={true}
                value={selectedGenders}
                onChange={setSelectedGenders} label={"genders"}
                valid={gendersError === undefined}
                errorMessage={gendersError}
            />
            <div className="flex flex-col gap-8 w-full items-center">
                <DualRangeInput
                    label={"age"}
                    min={minAgeRange}
                    max={maxAgeRange}
                    step={1}
                    value={[ageRange.lower, ageRange.higher]}
                    minStepsBetweenThumbs={1}
                    onChange={(value: [number, number]) => setAgeRange({ lower: value[0], higher: value[1] })}
                />
                <DualRangeInput
                    label={"distance (km)"}
                    min={minDistanceRange}
                    max={maxDistanceRange}
                    step={1}
                    value={[distanceRange.lower, distanceRange.higher]}
                    minStepsBetweenThumbs={1}
                    onChange={(value: [number, number]) => setDistanceRange({ lower: value[0], higher: value[1] })}
                />
                <Button className="max-w-xs" onClick={next}>next</Button>
            </div>
        </div>
    )
}

type ImagesStepProps = {
    navigate: (step: Step) => void;
    setImages?: (images: { src: string }[]) => void;
}
function ImagesStep({ navigate, setImages }: ImagesStepProps) {


    const validationSchema = z.array(z.object({
        src: z.url()
    })).min(1, { message: "please add at least one image" })
        .max(10, { message: "you can add up to 10 images" });

    const [images, setImagesState] = useState<{ src: string }[]>([]);
    const [imagesError, setImagesError] = useState<string | undefined>(undefined);
    function onNext() {
        const result = validationSchema.safeParse(images);
        if (!result.success) {
            setImagesError(result.error.issues[0].message);
            return;
        } else {
            setImagesError(undefined);
            setImages?.(images);
            navigate("success");
        }
    }

    return (
        <div className="flex flex-col gap-3 w-full items-center">
            <ImageGallery value={images} onChange={(newImages) => setImagesState(newImages)} valid={imagesError === undefined} errorMessage={imagesError} />
            <Button className="max-w-xs" onClick={onNext}>next</Button>
        </div>
    )
}

export default function RegisterForm({ className }: RegisterFormProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const [step, setStep] = useState<Step>("email");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [gender, setGender] = useState<Gender>(Gender.Male);
    const [birthDate, setBirthDate] = useState("");

    const [preferredGenders, setPreferredGenders] = useState<Gender[]>([]);
    const [preferredAgeRange, setPreferredAgeRange] = useState<{ lower: number, higher: number }>({ lower: 18, higher: 100 });
    const [preferredDistanceRange, setPreferredDistanceRange] = useState<{ lower: number, higher: number }>({ lower: 1, higher: 160 });
    const [images, setImages] = useState<{ src: string }[]>([]);

    const steps = [
        "email",
        "password",
        "profile",
        "dating preferences",
        "images",
    ]

    async function navigate(step: Step) {
        setStep(step);
        if (step === "success") {
            throw new Error("Not Implemented");
        }
    }

    return (
        <div className={`form flex flex-col gap-5 p-8 ${classNames} w-600px max-w-full`}>
            {step !== "account exists" && <ProgressBar steps={steps.length} progress={steps.indexOf(step) + 1} />}
            {step !== "email" && <span className="form-title">{step}</span>}
            {
                step === "email" && <EmailStep navigate={navigate} setEmail={setEmail} /> ||
                step === "password" && <PasswordStep navigate={navigate} setPassword={setPassword} /> ||
                step === "profile" && <ProfileStep navigate={navigate} setGender={setGender} setBirthDate={setBirthDate} /> ||
                step === "dating preferences" && <PreferencesStep navigate={navigate} onGendersChange={setPreferredGenders} onAgeRangeChange={setPreferredAgeRange} onDistanceRangeChange={setPreferredDistanceRange} /> ||
                step === "images" && <ImagesStep navigate={navigate} setImages={setImages} /> ||
                step === "account exists" && <AccountExistsStep />
            }
        </div>
    )

}