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
import { Gender, LengthUnit } from "@/src/shared/types/domain/value-objects";
import ProfileForm, { ProfileFormData, ProfileFormError } from "./ProfileForm";
import DatingPreferencesForm, { DatingPreferencesFormData, DatingPreferencesFormError } from "./DatingPreferencesForm";
import { ImagesFormData, ImagesFormError } from "./ImagesForm";
import { Range } from "@/src/shared/types/range";
import { CreateFile } from "@/src/shared/types/dtos/types";

export type RegisterFormProps = {
    className?: string;
}

type Step =
    "email" |
    "account exists" |
    "password" |
    "profile" |
    "preferences" |
    "images";

type EmailStepProps = {
    errorMessage?: string;
    finish: (accountExists: boolean) => void;
    setEmail: (email: string) => void;
    value?: string
    hidden?: boolean;
}

function EmailStep({ finish, setEmail, value, errorMessage, hidden }: EmailStepProps) {

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
        <div className={`${hidden ? "hidden" : ""} flex flex-col gap-3 items-center justify-center` }>
            <EmailInput value={email} onChange={setEmailState} valid={emailError === undefined} errorMessage={emailError} />
            {error && <span className="error-message">{error}</span>}
            <Button className="w-full" onClick={onNavigate}>next</Button>
        </div>
    )

}

function AccountExistsStep({ hidden }: { hidden: boolean }) {
    const router = useRouter();
    function onClick() {
        router.push("/public/login");
    }
    return (
        <Button className={`${hidden ? "hidden" : ""} w-full`} onClick={onClick}>login</Button>
    )
}

type PasswordStepProps = {
    errorMessage?: string;
    finish: () => void;
    setPassword: (password: string) => void;
    value?: string
    hidden?: boolean;
}
function PasswordStep({ finish, setPassword, value: initialPassword, errorMessage, hidden }: PasswordStepProps) {

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
        <div className={`${hidden ? "hidden" : ""} flex flex-col gap-3 items-center w-full`}>
            <TextInput label="password" type="password" value={password} onChange={(pw) => { setPasswordState(pw); clearErrors(); }} errorMessage={undefined} />
            <TextInput label="confirm password" type="password" value={confirmPassword} onChange={(pw) => { setConfirmPasswordState(pw); clearErrors(); }} errorMessage={error} />
            <Button className="max-w-xs" onClick={next}>next</Button>
        </div>
    )

}

type ProfileStepProps = {
    finish: () => void;
    setGender: (gender: Gender) => void;
    setBirthDate: (birthDate: string) => void;
    setUserName: (userName: string) => void;
    hidden?: boolean;
}
function ProfileStep({ finish, setGender, setBirthDate, setUserName, hidden }: ProfileStepProps) {


    console.log("rendering profile step");
    const validationSchema = z.object({
        userName: z.string({ message: "username is required" }).min(1, { message: "username is required" })
    });

    const year = new Date().getFullYear();
    const month = new Date().getMonth() + 1;
    const day = new Date().getDate();

    const minDate = `${year - 120}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;
    const maxDate = `${year - 18}-${month.toString().padStart(2, "0")}-${day.toString().padStart(2, "0")}`;

    const [profileData, setProfileData] = useState<ProfileFormData>({
        birthDateRange: { lower: minDate, upper: maxDate },
        birthDate: maxDate,
        gender: Gender.Male
    });

    const [profileError, setProfileError] = useState<ProfileFormError | undefined>(undefined);

    function onNext() {
        const result = validationSchema.safeParse(profileData);
        if (!result.success) {
            const properties = z.treeifyError(result.error).properties;
            setProfileError({ userName: properties?.userName?.errors?.[0] });
            return;
        }
        setProfileError(undefined);
        setGender(profileData.gender!);
        setBirthDate(profileData.birthDate!);
        setUserName(profileData.userName!);
        finish();
    }

    return (
        <div className={`${hidden ? "hidden" : ""} flex flex-col gap-3 items-center w-full`}>
            <ProfileForm data={profileData} onChange={setProfileData} error={profileError} />
            <Button className="max-w-xs" onClick={onNext}>next</Button>
        </div>
    )
}

type PreferencesStepProps = {
    onGendersChange?: (genders: Gender[]) => void;
    onAgeRangeChange?: (ageRange: Range<number>) => void;
    onDistanceChange?: (distance: number) => void;
    finish: () => void;
    hidden?: boolean;
}
function PreferencesStep({ onGendersChange, onAgeRangeChange, onDistanceChange, finish, hidden }: PreferencesStepProps) {


    const validationSchema = z.object({
        genders: z.array(z.enum(Gender)).min(1, { message: "please select at least one gender" }),
    });

    const minAgeRange = 18;
    const maxAgeRange = 100;
    const minDistanceRange = 1;
    const maxDistanceRange = 160;

    const [datingPreferencesData, setDatingPreferencesData] = useState<DatingPreferencesFormData>({
        possibleAgeRange: { lower: minAgeRange, upper: maxAgeRange },
        possibleDistanceRange: { lower: minDistanceRange, upper: maxDistanceRange },
        genders: [],
        ageRange: { lower: minAgeRange, upper: maxAgeRange },
        distance: maxDistanceRange
    });
    const [datingPreferencesError, setDatingPreferencesError] = useState<DatingPreferencesFormError | undefined>(undefined);

    function next() {

        const result = validationSchema.safeParse(datingPreferencesData);

        if (!result.success) {
            const properties = z.treeifyError(result.error).properties;
            setDatingPreferencesError({ genders: properties?.genders?.errors?.[0] });
            return;
        }

        onGendersChange?.(datingPreferencesData.genders!);
        onAgeRangeChange?.(datingPreferencesData.ageRange!);
        onDistanceChange?.(datingPreferencesData.distance!);
        finish();

    }

    return (
        <div className={`${hidden ? "hidden" : ""} flex flex-col gap-10 w-full items-center`}>
            <DatingPreferencesForm data={datingPreferencesData} onChange={setDatingPreferencesData} error={datingPreferencesError} />
            <Button className="max-w-xs" onClick={next}>next</Button>
        </div>
    )
}

type ImagesStepProps = {
    finish?: (images: CreateFile[]) => void;
    images?: CreateFile[]
    hidden?: boolean;
}
function ImagesStep({ finish: finish, hidden }: ImagesStepProps) {

    const validationSchema = z.array(z.object({
        url: z.url(),
        mimeType: z.string(),
    })).min(2, { message: "please add at least two images" })
        .max(10, { message: "you can add up to 10 images" });

    const [imagesData, setImagesData] = useState<ImagesFormData | undefined>(undefined);
    const [imagesError, setImagesError] = useState<ImagesFormError | undefined>(undefined);

    function onNext() {
        const result = validationSchema.safeParse(imagesData?.images);
        if (!result.success) {
            setImagesError({ images: result.error.issues[0].message });
            return;
        } else {
            finish?.(imagesData!.images);
        }
    }

    return (
        <div className={`${hidden ? "hidden" : ""} flex flex-col gap-3 w-full items-center pt-3`}>
            <ImageGallery
                data={imagesData}
                error={imagesError}
                onChange={(newImages) => setImagesData(prev => ({ ...prev, images: newImages.map((image, index) => ({ ...image, order: index })) }))}
            />
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
    const [data, setData] = useState<{
        email?: string;
        userName?: string;
        password?: string;
        gender?: Gender;
        birthDate?: string;
        datingPreferences?: {
            interestedInGenders?: Gender[];
            ageRange?: Range<number>;
            maxDistance?: number;
        },
        images?: CreateFile[];
    }>({});


    const steps: Step[] = [
        "email",
        "password",
        "profile",
        "preferences",
        "images"
    ]

    async function navigate(step: Step) {

        setStep(step);

    }

    async function finish(images: CreateFile[]) {

        setData(prev => ({ ...prev, images }));

        const response = await register({
            email: data.email!,
            userName: data.userName!,
            password: data.password!,
            gender: data.gender!,
            birthDate: data.birthDate!,
            datingPreferences: {
                interestedInGenders: data.datingPreferences?.interestedInGenders!,
                ageRangeMax: data.datingPreferences?.ageRange?.upper!,
                ageRangeMin: data.datingPreferences?.ageRange!?.lower,
                maximumDistance: { value: data.datingPreferences?.maxDistance!, unit: LengthUnit.Kilometers },
            },
            files: images
        });

        console.log("register response", response);

        if (response.success) {
            router.push("/public/login");
        } else {
            console.log("register error reason", response.reason);
        }
    }

    return (
        <div className={`form flex flex-col gap-3 p-8 ${classNames} w-600px max-w-full`}>
            {step !== "account exists" && <ProgressBar steps={steps} progress={steps.indexOf(step) + 1} onClick={(index) => { navigate(steps[index]) }} />}
            {step === "account exists" && <span className="form-title">{step}</span>}
            <EmailStep
                hidden={step !== "email"}
                finish={(accountExists) => { accountExists ? navigate("account exists") : navigate("password") }}
                setEmail={(email) => setData(prev => ({ ...prev, email }))}
                value={data.email}
            />
            <PasswordStep
                hidden={step !== "password"}
                finish={() => navigate("profile")}
                setPassword={(password) => setData(prev => ({ ...prev, password }))}
                value={data.password}
            />
            <ProfileStep
                hidden={step !== "profile"}
                finish={() => navigate("preferences")}
                setUserName={(userName) => setData(prev => ({ ...prev, userName }))}
                setGender={(gender) => setData(prev => ({ ...prev, gender }))}
                setBirthDate={(birthDate) => setData(prev => ({ ...prev, birthDate }))}
            />
            <PreferencesStep
                hidden={step !== "preferences"}
                finish={() => navigate("images")}
                onGendersChange={(interestedInGenders) => setData(prev => ({ ...prev, datingPreferences: { ...prev.datingPreferences, interestedInGenders } }))}
                onAgeRangeChange={(ageRange) => setData(prev => ({ ...prev, datingPreferences: { ...prev.datingPreferences, ageRange } }))}
                onDistanceChange={(maxDistance) => setData(prev => ({ ...prev, datingPreferences: { ...prev.datingPreferences, maxDistance } }))}
            />
            <ImagesStep
                hidden={step !== "images"}
                finish={finish}
                images={data.images}
            />
            <AccountExistsStep
                hidden={step !== "account exists"}
            />
        </div>
    )

}