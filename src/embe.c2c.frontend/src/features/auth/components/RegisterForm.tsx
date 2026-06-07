"use client";

import Button from "@/src/shared/components/buttons/Button";
import { EmailInput } from "@/src/shared/components/inputs/email-input/EmailInput";
import { useState } from "react";
import ProgressBar from "@/src/shared/components/progress-bar/ProgressBar";
import ImageGallery from "@/src/shared/components/inputs/image/gallery/ImageGallery";
import * as z from "zod";
import { accountExists as accountExists } from "../actions/account-exists/actions";
import { useRouter } from "next/navigation";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import { register } from "@/src/features/auth/actions/register/actions";
import { FileDetails, Gender, LengthUnit } from "@/src/shared/types/domain/value-objects";
import ProfileForm, { ProfileFormData } from "./ProfileForm";
import DatingPreferencesForm, { DatingPreferencesFormData } from "./DatingPreferencesForm";
import { ImagesFormData } from "./ImagesForm";

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
    const [error, setError] = useState<string | undefined>(undefined);

    async function onNavigate() {
        const result = await emailSchema.safeParseAsync(email);
        if (!result.success) {
            setEmailError(result.error.issues[0].message);
            return;
        } else {
            const response = await accountExists(email!);
            if (response.success) {
                if (response.value!) {
                    navigate("account exists")
                } else {
                    setEmail(result.data);
                    navigate("password");
                }
            } else {
                setError("an unknown error occurred");
            }
        }
    }

    return (
        <div className="flex flex-col gap-3">
            <EmailInput value={email} onChange={setEmailState} valid={emailError === undefined} errorMessage={emailError} />
            {error && <span className="error-message">{error}</span>}
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

    function clearErrors() {
        setError(undefined);
    }

    return (
        <div className="flex flex-col gap-3">
            <TextInput label="password" type="password" value={password} onChange={(pw) => { setPasswordState(pw); clearErrors(); }} valid={error === undefined} errorMessage={undefined} />
            <TextInput label="confirm password" type="password" value={confirmPassword} onChange={(pw) => { setConfirmPasswordState(pw); clearErrors(); }} valid={error === undefined} errorMessage={error} />
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
    const [profileData, setProfileData] = useState<ProfileFormData>({
        birthDateRange: { lower: minDate, higher: maxDate },
        birthDate: maxDate,
        gender: Gender.Male
    });

    function onNext() {
        setGender(profileData.gender);
        setBirthDate(profileData.birthDate);
        navigate("dating preferences");
    }

    return (
        <div className="flex flex-col gap-3 items-center w-full">
            <ProfileForm data={profileData} onChange={setProfileData} />
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
        genders: z.array(z.enum(Gender)).min(1, { message: "please select at least one gender" }),
    });

    const minAgeRange = 18;
    const maxAgeRange = 100;
    const minDistanceRange = 0;
    const maxDistanceRange = 160;

    const [datingPreferencesData, setDatingPreferencesData] = useState<DatingPreferencesFormData>({
        genders: [],
        ageRange: { lower: minAgeRange, higher: maxAgeRange },
        distanceRange: { lower: minDistanceRange, higher: maxDistanceRange },
        gendersError: undefined
    });

    function next() {

        const result = validationSchema.safeParse(datingPreferencesData);

        if (!result.success) {
            const properties = z.treeifyError(result.error).properties;
            setDatingPreferencesData(prev => ({ ...prev, gendersError: properties?.genders?.errors?.[0] }));
            return;
        }

        onGendersChange?.(datingPreferencesData.genders);
        onAgeRangeChange?.(datingPreferencesData.ageRange);
        onDistanceRangeChange?.(datingPreferencesData.distanceRange);
        navigate("images");
    }

    return (
        <div className="flex flex-col gap-3 w-full items-center">
            <DatingPreferencesForm data={datingPreferencesData} onChange={setDatingPreferencesData} />
            <Button className="max-w-xs" onClick={next}>next</Button>
        </div>
    )
}

type ImagesStepProps = {
    finish?: (images: FileDetails[]) => void;
}
function ImagesStep({ finish }: ImagesStepProps) {

    const validationSchema = z.array(z.object({
        url: z.url(),
        mimeType: z.string(),
    })).min(2, { message: "please add at least two images" })
        .max(10, { message: "you can add up to 10 images" });

    const [imagesData, setImagesData] = useState<ImagesFormData>({
        images: [],
        imagesError: undefined
    });

    function onNext() {
        const result = validationSchema.safeParse(imagesData.images);
        if (!result.success) {
            setImagesData(prev => ({ ...prev, imagesError: result.error.issues[0].message }));
            return;
        } else {
            finish?.(imagesData.images);
        }
    }

    return (
        <div className="flex flex-col gap-3 w-full items-center">
            <ImageGallery value={imagesData.images} onChange={(newImages) => setImagesData(prev => ({ ...prev, images: newImages }))} valid={imagesData.imagesError === undefined} errorMessage={imagesData.imagesError} />
            <Button className="max-w-xs" onClick={onNext}>next</Button>
        </div>
    )

}

export default function RegisterForm({ className }: RegisterFormProps) {

    const router = useRouter();
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

    const steps = [
        "email",
        "password",
        "profile",
        "dating preferences",
        "images",
    ]

    async function navigate(step: Step) {

        setStep(step);
    }

    async function finish(images: FileDetails[]) {
        const response = await register({
            email,
            password,
            birthDate,
            gender,
            datingPreferences: {
                interestedInGenders: preferredGenders,
                ageRangeMin: preferredAgeRange.lower,
                ageRangeMax: preferredAgeRange.higher,
                maximumDistance: {
                    value: preferredDistanceRange.higher,
                    unit: LengthUnit.Kilometers
                }
            },
            files: images
        });

        if (response.success) {
            router.push("/login");
        } else {

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
                step === "images" && <ImagesStep finish={finish} /> ||
                step === "account exists" && <AccountExistsStep />
            }
        </div>
    )

}