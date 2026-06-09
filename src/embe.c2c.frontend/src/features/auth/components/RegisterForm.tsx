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
import { RegisterUserFailureReason } from "../actions/register/types";

export type RegisterFormProps = {
    className?: string;
}

type Step =
    "email" |
    "account exists" |
    "password" |
    "profile" |
    "dating preferences" |
    "images";

type EmailStepProps = {
    errorMessage?: string;
    finish: (accountExists: boolean) => void;
    setEmail: (email: string) => void;
    value?: string
}

function EmailStep({ finish, setEmail, value, errorMessage }: EmailStepProps) {

    const emailSchema = z.email({ message: "please enter a valid email" });
    const [email, setEmailState] = useState<string | undefined>(value);
    const [emailError, setEmailError] = useState<string | undefined>(undefined);
    const [error, setError] = useState<string | undefined>(errorMessage);

    async function onNavigate() {
        const result = await emailSchema.safeParseAsync(email);
        if (!result.success) {
            setEmailError(result.error.issues[0].message);
            return;
        } else {
            const response = await accountExists(email!);
            if (response.success) {
                if (response.value!) {
                    finish(true)
                } else {
                    setEmail(result.data);
                    finish(false);
                }
            } else {
                setError("an unknown error occurred");
            }
        }
    }

    return (
        <div className="flex flex-col gap-3 items-center justify-center">
            <EmailInput value={email} onChange={setEmailState} valid={emailError === undefined} errorMessage={emailError} />
            {error && <span className="error-message">{error}</span>}
            <Button className="max-w-xs" onClick={onNavigate}>next</Button>
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
    errorMessage?: string;
    finish: () => void;
    setPassword: (password: string) => void;
    value?: string
}
function PasswordStep({ finish, setPassword, value: initialPassword, errorMessage }: PasswordStepProps) {

    const validationSchema = z.object({
        password: z.string(),
        confirmPassword: z
            .string()
            .min(8, { message: "password must be at least 8 characters long" })
            .refine((value) => /[A-Z]/.test(value), { message: "password must contain at least one uppercase letter" })
            .refine((value) => /[a-z]/.test(value), { message: "password must contain at least one lowercase letter" })
            .refine((value) => /[0-9]/.test(value), { message: "password must contain at least one number" })
    }).refine((data) => data.password === data.confirmPassword, {
        message: "passwords do not match",
    });

    const [password, setPasswordState] = useState<string | undefined>(initialPassword);
    const [confirmPassword, setConfirmPasswordState] = useState<string | undefined>(initialPassword);
    const [error, setError] = useState<string | undefined>(errorMessage);

    function next() {
        const result = validationSchema.safeParse({ password, confirmPassword });
        if (!result.success) {
            setError(result.error.issues[0].message);
            return;
        } else {
            setError(undefined);
            setPassword(password!);
            finish();
        }
    }

    function clearErrors() {
        setError(undefined);
    }

    return (
        <div className="flex flex-col gap-3 items-center w-full">
            <TextInput label="password" type="password" value={password} onChange={(pw) => { setPasswordState(pw); clearErrors(); }} valid={true} errorMessage={undefined} />
            <TextInput label="confirm password" type="password" value={confirmPassword} onChange={(pw) => { setConfirmPasswordState(pw); clearErrors(); }} valid={error === undefined} errorMessage={error} />
            <Button className="max-w-xs" onClick={next}>next</Button>
        </div>
    )

}

type ProfileStepProps = {
    errorMessage?: string;
    finish: () => void;
    setGender: (gender: Gender) => void;
    setBirthDate: (birthDate: string) => void;
    gender?: Gender;
    birthDate?: string;
}
function ProfileStep({ finish, setGender, setBirthDate, errorMessage, gender: initialGender, birthDate: initialBirthDate }: ProfileStepProps) {

    const year = new Date().getFullYear();
    const month = new Date().getMonth() + 1;
    const day = new Date().getDate();

    const minDate = `${year - 120}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;
    const maxDate = `${year - 18}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;
    const [profileData, setProfileData] = useState<ProfileFormData>({
        birthDateRange: { lower: minDate, higher: maxDate },
        birthDate: initialBirthDate || maxDate,
        gender: initialGender || Gender.Male
    });

    function onNext() {
        setGender(profileData.gender);
        setBirthDate(profileData.birthDate);
        finish();
    }

    return (
        <div className="flex flex-col gap-3 items-center w-full">
            <ProfileForm data={profileData} onChange={setProfileData} errorMessage={errorMessage} />
            <Button className="max-w-xs" onClick={onNext}>next</Button>
        </div>
    )
}

type PreferencesStepProps = {
    onGendersChange?: (genders: Gender[]) => void;
    onAgeRangeChange?: (ageRange: { lower: number, higher: number }) => void;
    onDistanceRangeChange?: (distanceRange: { lower: number, higher: number }) => void;
    finish: () => void;
    errorMessage?: string;
    genders?: Gender[],
    ageRange?: { lower: number, higher: number },
    distanceRange?: { lower: number, higher: number }
}
function PreferencesStep({ onGendersChange, onAgeRangeChange, onDistanceRangeChange, finish, errorMessage,
    genders: initialGenders,
    ageRange: initialAgeRange,
    distanceRange: initialDistanceRange
}: PreferencesStepProps) {

    const validationSchema = z.object({
        genders: z.array(z.enum(Gender)).min(1, { message: "please select at least one gender" }),
    });

    const minAgeRange = 18;
    const maxAgeRange = 100;
    const minDistanceRange = 0;
    const maxDistanceRange = 160;

    const [datingPreferencesData, setDatingPreferencesData] = useState<DatingPreferencesFormData>({
        genders: initialGenders || [],
        ageRange: initialAgeRange || { lower: minAgeRange, higher: maxAgeRange },
        distanceRange: initialDistanceRange || { lower: minDistanceRange, higher: maxDistanceRange },
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
        finish();

    }

    return (
        <div className="flex flex-col gap-10 w-full items-center">
            <DatingPreferencesForm data={datingPreferencesData} onChange={setDatingPreferencesData} />
            <Button className="max-w-xs" onClick={next}>next</Button>
        </div>
    )
}

type ImagesStepProps = {
    finish?: (images: FileDetails[]) => void;
    errorMessage?: string;
    images?: FileDetails[]
}
function ImagesStep({ finish: finish, errorMessage: initialErrorMessage, images: initialImages }: ImagesStepProps) {

    const validationSchema = z.array(z.object({
        url: z.url(),
        mimeType: z.string(),
    })).min(2, { message: "please add at least two images" })
        .max(10, { message: "you can add up to 10 images" });

    const [imagesData, setImagesData] = useState<ImagesFormData>({
        images: initialImages || [],
        imagesError: initialErrorMessage
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

    const isValid = imagesData.imagesError === undefined && initialErrorMessage === undefined;
    const errorMessage = imagesData.imagesError || initialErrorMessage;
    return (
        <div className="flex flex-col gap-3 w-full items-center">
            <ImageGallery value={imagesData.images} onChange={(newImages) => setImagesData(prev => ({ ...prev, images: newImages }))} valid={isValid} errorMessage={errorMessage} />
            <Button className="max-w-xs" onClick={onNext}>finish</Button>
        </div>
    )

}

export default function RegisterForm({ className }: RegisterFormProps) {

    const router = useRouter();
    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const [step, setStep] = useState<Step>("email");
    const [email, setEmail] = useState<string | undefined>(undefined);
    const [password, setPassword] = useState<string | undefined>(undefined);
    const [gender, setGender] = useState<Gender>(Gender.Male);
    const [birthDate, setBirthDate] = useState<string | undefined>(undefined);

    const [preferredGenders, setPreferredGenders] = useState<Gender[]>([]);
    const [preferredAgeRange, setPreferredAgeRange] = useState<{ lower: number, higher: number }>({ lower: 18, higher: 100 });
    const [preferredDistanceRange, setPreferredDistanceRange] = useState<{ lower: number, higher: number }>({ lower: 0, higher: 160 });
    const [images, setImages] = useState<FileDetails[]>([]);

    const [errorMessage, setErrorMessage] = useState<string | undefined>(undefined);

    const steps = [
        "email",
        "password",
        "profile",
        "dating preferences",
        "images"
    ]

    async function navigate(step: Step) {

        setStep(step);
        setErrorMessage(undefined);

    }

    async function finish(images: FileDetails[]) {

        setImages(images);

        const response = await register({
            email: email!,
            password: password!,
            birthDate: birthDate!,
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

        console.log("register response", response);

        if (response.success) {
            router.push("/login");
        } else {
            switch (response.reason) {
                case RegisterUserFailureReason.EmailAlreadyExists:
                    setErrorMessage(response.message?.toLowerCase() || "email already exists");
                    setStep("account exists");
                    break;
                case RegisterUserFailureReason.DomainError:
                    setErrorMessage(response.message?.toLowerCase() || "a domain error occurred");
                    setStep("images");
                    break;
                case RegisterUserFailureReason.WeakPassword:
                    setErrorMessage(response.message?.toLowerCase() || "the password is too weak");
                    setStep("password");
                    break;
                case RegisterUserFailureReason.Unknown:
                    setErrorMessage(response.message?.toLowerCase() || "an unknown error occurred");
                    setStep("images");
                    break;
                case RegisterUserFailureReason.UnknownError:
                    setErrorMessage(response.message?.toLowerCase() || "an unknown error occurred");
                    setStep("images");
                    break;
                default:
                    setErrorMessage("an unknown error occurred");
                    setStep("images");
                    break;
            }
        }
    }

    return (
        <div className={`form flex flex-col gap-5 p-8 ${classNames} w-600px max-w-full`}>
            {step !== "account exists" && <ProgressBar steps={steps.length} progress={steps.indexOf(step) + 1} />}
            {step !== "email" && <span className="form-title">{step}</span>}
            {
                step === "email" &&
                <EmailStep
                    finish={(accountExists) => { accountExists ? navigate("account exists") : navigate("password") }}
                    setEmail={setEmail}
                    errorMessage={errorMessage}
                    value={email}
                /> ||
                step === "password" && <PasswordStep finish={() => navigate("profile")} setPassword={setPassword} errorMessage={errorMessage} value={password} /> ||
                step === "profile" && <ProfileStep finish={() => navigate("dating preferences")} setGender={setGender} setBirthDate={setBirthDate} errorMessage={errorMessage} gender={gender} birthDate={birthDate} /> ||
                step === "dating preferences" && <PreferencesStep finish={() => navigate("images")} onGendersChange={setPreferredGenders} onAgeRangeChange={setPreferredAgeRange} onDistanceRangeChange={setPreferredDistanceRange} errorMessage={errorMessage} genders={preferredGenders} ageRange={preferredAgeRange} distanceRange={preferredDistanceRange} /> ||
                step === "images" && <ImagesStep finish={finish} errorMessage={errorMessage} images={images} /> ||
                step === "account exists" && <AccountExistsStep />
            }
        </div>
    )

}