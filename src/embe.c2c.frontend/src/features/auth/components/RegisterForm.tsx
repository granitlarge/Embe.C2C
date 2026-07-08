"use client";

import Button from "@/src/shared/components/buttons/Button";
import { EmailInput } from "@/src/shared/components/inputs/email-input/EmailInput";
import { useState } from "react";
import ProgressBar from "@/src/shared/components/progress-bar/ProgressBar";
import ImageGalleryInput from "@/src/shared/components/inputs/image/gallery/ImageGalleryInput";
import * as z from "zod";
import { accountExists as accountExists } from "../actions/account-exists/actions";
import { useRouter } from "next/navigation";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import { register } from "@/src/features/auth/actions/register/actions";
import { Gender } from "@/src/shared/types/domain/value-objects";
import SearchProfileBuilderForm, { SearchProfileBuilderFormData, SearchProfileBuilderFormError } from "./SearchProfileBuilderForm";
import { ImagesFormData, ImagesFormError } from "./ImagesForm";
import { Range } from "@/src/shared/types/range";
import { CreateFile } from "@/src/shared/types/dtos/types";
import Surface from "@/src/shared/components/surfaces/Surface";
import BasicProfileForm, { BasicProfileFormData, BasicProfileFormError } from "./BasicProfileForm";
import { getValidBirthdateRange } from "@/src/shared/time";

type Step =
    "email" |
    "account exists" |
    "password" |
    "profile";

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
        <Surface className={`form ${hidden ? "hidden" : ""}`} padding="none">
            <EmailInput
                value={email}
                onChange={setEmailState}
                errorMessage={emailError}
            />
            {error && <span className="text-(--error-fc)">{error}</span>}
            <Button onClick={onNavigate}>next</Button>
        </Surface>
    )

}

function AccountExistsStep({ hidden }: { hidden: boolean }) {
    const router = useRouter();
    function onClick() {
        router.push("/public/login");
    }
    return (
        <Button className={`${hidden ? "hidden" : ""}`} onClick={onClick}>login</Button>
    )
}

type PasswordStepProps = {
    errorMessage?: string;
    finish: () => void;
    setPassword: (password: string) => void;
    value?: string
    hidden?: boolean;
}
function PasswordStep({
    finish,
    setPassword,
    value: initialPassword,
    errorMessage,
    hidden,
}: PasswordStepProps) {

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
        <Surface className={`${hidden ? "hidden" : ""} form`} padding="none">
            <TextInput
                label="password"
                type="password"
                initialValue={password}
                onBlur={(pw) => { setPasswordState(pw); clearErrors(); }}
                errorMessage={undefined}
            />
            <TextInput
                label="confirm password"
                type="password"
                initialValue={confirmPassword}
                onBlur={(pw) => { setConfirmPasswordState(pw); clearErrors(); }}
                errorMessage={error}
            />
            <Button onClick={next}>next</Button>
        </Surface>
    )
}

type BasicProfileStepProps = {
    finish: (birthDate: string, alias: string) => void;
    hidden?: boolean;
}
function BasicProfileStep({ finish, hidden }: BasicProfileStepProps) {

    const validationSchema = z.object({
        alias: z.string({ message: "alias is required" }).min(1, { message: "alias is required" })
    });

    const { lower, upper } = getValidBirthdateRange(18, 120);

    const [profileData, setProfileData] = useState<BasicProfileFormData>({
        birthDateRange: { lower, upper },
        birthDate: upper,
        alias: ""
    });

    const [profileError, setProfileError] = useState<BasicProfileFormError | undefined>(undefined);

    function onNext() {
        const result = validationSchema.safeParse(profileData);
        if (!result.success) {
            const properties = z.treeifyError(result.error).properties;
            setProfileError({ alias: properties?.alias?.errors?.[0] });
            return;
        }
        setProfileError(undefined);
        finish(profileData.birthDate!, profileData.alias!);
    }

    return (
        <BasicProfileForm
            className={`${hidden ? "hidden" : ""} form`}
            data={profileData}
            onChange={setProfileData}
            error={profileError}
        >
            <Button onClick={onNext}>finish</Button>
        </BasicProfileForm>
    )

}

type SearchProfileStepProps = {
    onGendersChange?: (genders: Gender[]) => void;
    onAgeRangeChange?: (ageRange: Range<number>) => void;
    onDistanceChange?: (distance: number) => void;
    finish: () => void;
    hidden?: boolean;
}
function SearchProfileStep({ onGendersChange, onAgeRangeChange, onDistanceChange, finish, hidden }: SearchProfileStepProps) {

    const validationSchema = z.object({
        genders: z.array(z.enum(Gender)).min(1, { message: "please select at least one gender" }),
    });

    const minAgeRange = 18;
    const maxAgeRange = 100;
    const minDistanceRange = 1;
    const maxDistanceRange = 160;

    const [datingPreferencesData, setDatingPreferencesData] = useState<SearchProfileBuilderFormData>({
        possibleAgeRange: { lower: minAgeRange, upper: maxAgeRange },
        possibleDistanceRange: { lower: minDistanceRange, upper: maxDistanceRange },
        genders: [],
        ageRange: { lower: minAgeRange, upper: maxAgeRange },
        distance: maxDistanceRange
    });
    const [datingPreferencesError, setDatingPreferencesError] = useState<SearchProfileBuilderFormError | undefined>(undefined);

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
        <SearchProfileBuilderForm className={`${hidden ? "hidden" : ""}`} data={datingPreferencesData} onChange={setDatingPreferencesData} error={datingPreferencesError} >
            <Button className="max-w-xs" onClick={next}>next</Button>
        </SearchProfileBuilderForm>
    )
}

type ImagesStepProps = {
    finish?: (images: CreateFile[]) => void;
    images?: CreateFile[]
    hidden?: boolean;
}
function ImagesStep({ finish: finish, hidden, }: ImagesStepProps) {

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
            <ImageGalleryInput
                data={imagesData}
                error={imagesError}
                onChange={(newImages) => setImagesData(prev => ({ ...prev, images: newImages.map((image, index) => ({ ...image, order: index })) }))}
            />
            <Button className="max-w-xs" onClick={onNext}>finish</Button>
        </div>
    )

}
export type RegisterFormProps = {
    className?: string;
}
export default function RegisterForm({ className }: RegisterFormProps) {

    const router = useRouter();
    const classNames = [
        className
    ].filter(Boolean).join(" ");

    const [step, setStep] = useState<Step>("email");
    const [data, setData] = useState<{
        email?: string;
        password?: string;
    }>({});

    const steps: Step[] = [
        "email",
        "password",
        "profile",
    ]

    async function navigate(step: Step) {
        setStep(step);
    }

    async function finish(birthDate: string, alias: string) {

        const response = await register({
            email: data.email!,
            alias: alias!,
            password: data.password!,
            birthDate: birthDate!,
        });

        if (response.success) {
            router.push("/public/login");
        } else {
            console.log(response);
            throw new Error("not implemented");
        }

    }

    return (
        <Surface className={`form flex flex-col gap-5 p-0 ${classNames} w-600px max-w-full`} variant="secondary">
            {step !== "account exists" && <ProgressBar steps={steps} progress={steps.indexOf(step) + 1} onClick={(index) => { navigate(steps[index]) }} />}
            {step === "account exists" && <span className="mx-auto">{step}</span>}
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
            <BasicProfileStep
                hidden={step !== "profile"}
                finish={finish}
            />
            <AccountExistsStep
                hidden={step !== "account exists"}
            />
        </Surface>
    )
}