"use client";

import Button from "@/src/components/buttons/Button";
import { EmailInput } from "@/src/components/inputs/email-input/EmailInput";
import { useState } from "react";
import { checkEmailExists } from "../apis/EmailExists";
import DateInput from "@/src/components/inputs/date-input/DateInput";
import SelectInput from "@/src/components/inputs/select-input/SelectInput";
import { Gender } from "@/src/shared/types/gender";
import DualRangeInput from "@/src/components/inputs/dual-range-input/DualRangeInput";
import ProgressBar from "@/src/components/progress-bar/ProgressBar";

export type RegisterFormProps = {
    className?: string;
}

type Step =
    "email" |
    "account exists" |
    "profile" |
    "dating preferences" |
    "images" |
    "success";

function EmailStep({ advance, setEmail }: { advance: (step: Step) => void, setEmail: (email: string) => void }) {

    const [email, setEmailState] = useState("");
    async function onNext(email: string) {
        setEmail(email);
        const emailExists = await checkEmailExists(email);
        if (emailExists) {
            advance("account exists");
        } else {
            advance("profile");
        }
    }

    return (
        <div className="flex flex-col gap-3">
            <EmailInput required value={email} onChange={setEmailState} />
            <Button onClick={() => onNext(email)}>next</Button>
        </div>
    )
}

function AccountExistsStep() {
    return (
        <></>
    )
}

type ProfileStepProps = {
    advance: (step: Step) => void;
    setGender: (gender: Gender) => void;
    setBirthDate: (birthDate: string) => void;
}
function ProfileStep({ advance, setGender, setBirthDate }: ProfileStepProps) {

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
        advance("dating preferences");
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
            <Button onClick={onNext}>next</Button>
        </div>
    )
}

type PreferencesStepProps = {
    onGendersChange?: (genders: Gender[]) => void;
    onAgeRangeChange?: (ageRange: { lower: number, higher: number }) => void;
    onDistanceRangeChange?: (distanceRange: { lower: number, higher: number }) => void;
    advance: (step: Step) => void;
}
function PreferencesStep({ onGendersChange, onAgeRangeChange, onDistanceRangeChange, advance }: PreferencesStepProps) {

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
    const minDistanceRange = 1;
    const maxDistanceRange = 160;
    const [distanceRange, setDistanceRange] = useState<{ lower: number, higher: number }>({ lower: minDistanceRange, higher: maxDistanceRange });

    function next() {
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
        advance("images");
    }

    return (
        <div className="flex flex-col gap-3 w-full items-center">
            <SelectInput options={genders} multiple={true} value={selectedGenders} onChange={setSelectedGenders} label={"genders"} />
            <DualRangeInput
                label={"age range"}
                min={minAgeRange}
                max={maxAgeRange}
                step={1}
                value={[ageRange.lower, ageRange.higher]}
                minStepsBetweenThumbs={1}
                onChange={(value: [number, number]) => setAgeRange({ lower: value[0], higher: value[1] })}
            />
            <DualRangeInput
                label={"distance range (km)"}
                min={minDistanceRange}
                max={maxDistanceRange}
                step={1}
                value={[distanceRange.lower, distanceRange.higher]}
                minStepsBetweenThumbs={1}
                onChange={(value: [number, number]) => setDistanceRange({ lower: value[0], higher: value[1] })}
            />
            <Button onClick={next}>next</Button>
        </div>
    )

}

function ImagesStep() {
    return (
        <></>
    )
}

export default function RegisterForm({ className }: RegisterFormProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const [email, setEmail] = useState("");
    const [gender, setGender] = useState<Gender>(Gender.Male);
    const [birthDate, setBirthDate] = useState("");
    const [step, setStep] = useState<Step>("email");

    const [preferredGenders, setPreferredGenders] = useState<Gender[]>([]);
    const [preferredAgeRange, setPreferredAgeRange] = useState<{ lower: number, higher: number }>({ lower: 18, higher: 100 });
    const [preferredDistanceRange, setPreferredDistanceRange] = useState<{ lower: number, higher: number }>({ lower: 1, higher: 160 });

    const steps = [
        "email",
        "account exists",
        "profile",
        "dating preferences",
        "images",
        "success"
    ]

    return (
        <div className={`form flex flex-col gap-5 p-5 ${classNames}`}>
            <span className="form-title">{step}</span>
            <ProgressBar steps={steps.length} progress={steps.indexOf(step) + 1} />
            {
                step === "email" && <EmailStep advance={setStep} setEmail={setEmail} /> ||
                step === "profile" && <ProfileStep advance={setStep} setGender={setGender} setBirthDate={setBirthDate} /> ||
                step === "dating preferences" && <PreferencesStep advance={setStep} onGendersChange={setPreferredGenders} onAgeRangeChange={setPreferredAgeRange} onDistanceRangeChange={setPreferredDistanceRange} /> ||
                step === "images" && <ImagesStep />
            }
        </div>
    )

}