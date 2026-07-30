"use client";

import Button from "@/src/shared/components/buttons/Button";
import { EmailInput } from "@/src/shared/components/inputs/email-input/EmailInput";
import { useState } from "react";
import ProgressBar from "@/src/shared/components/progress-bar/ProgressBar";
import * as z from "zod";
import { accountExists as accountExists } from "../actions/account-exists/actions";
import TextInput from "@/src/shared/components/inputs/text-input/TextInput";
import { register } from "@/src/features/auth/actions/register/actions";
import { Gender, Location } from "@/src/shared/types/domain/value-objects";
import Surface from "@/src/shared/components/surfaces/Surface";
import BasicProfileForm, { BasicProfileFormData, BasicProfileFormError } from "./BasicProfileForm";
import { getValidBirthdateRange } from "@/src/shared/time";
import { useRouter } from "nextjs-toploader/app";
import { Routes } from "@/src/shared/routes";
import { PasswordValidationRules } from "@/src/shared/validation";
import { ImageData } from "../../me/components/MyInfoForm";
import ImageGalleryInput, { ImageGalleryInputData, ImageGalleryInputError } from "@/src/shared/components/inputs/image/gallery/ImageGalleryInput";
import { Image } from "@/src/shared/components/inputs/image/gallery/ImageGalleryInput";
import { getBase64EncodedData } from "@/src/shared/encoding";

type Step =
    "email" |
    "account exists" |
    "password" |
    "profile" | 
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
                throw new Error("not implemented");
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
            {errorMessage && <span className="text-(--error-fc)">{errorMessage}</span>}
            <Button intent="navigate" onClick={onNavigate}>next</Button>
        </Surface>
    )

}

function AccountExistsStep({ hidden }: { hidden: boolean }) {
    const router = useRouter();
    function onClick() {
        router.push(Routes.public.login);
    }
    return (
        <Button intent="navigate" className={`${hidden ? "hidden" : ""}`} onClick={onClick}>login</Button>
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
        confirmPassword: PasswordValidationRules
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
                value={password}
                onBlur={(pw) => { setPasswordState(pw); clearErrors(); }}
                errorMessage={undefined}
            />
            <TextInput
                label="confirm password"
                type="password"
                value={confirmPassword}
                onBlur={(pw) => { setConfirmPasswordState(pw); clearErrors(); }}
                errorMessage={error}
            />
            <Button intent="navigate" onClick={next}>next</Button>
        </Surface>
    )
}

type BasicProfileStepProps = {
    next: (birthDate: string, alias: string, location: Location | undefined, gender: Gender) => void;
    hidden?: boolean;
}
function BasicProfileStep({ next: finish, hidden }: BasicProfileStepProps) {

    const validationSchema = z.object({
        alias: z.string({ message: "alias is required" }).min(1, { message: "alias is required" }),
        birthDate: z.string({ message: "birth date is required" }).min(1),
        gender: z.enum(Gender, {error: "gender is required"})
    });

    const { lower, upper } = getValidBirthdateRange(18, 120);

    const [profileData, setProfileData] = useState<BasicProfileFormData>({
        birthDateRange: { lower, upper },
        birthDate: upper,
        alias: "",
        gender: Gender.Male,
    });

    const [profileError, setProfileError] = useState<BasicProfileFormError | undefined>(undefined);

    function onNext() {
        const result = validationSchema.safeParse(profileData);
        if (!result.success) {
            const properties = z.treeifyError(result.error).properties;
            setProfileError({
                alias: properties?.alias?.errors?.[0],
                birthDate: properties?.birthDate?.errors?.[0],
                gender: properties?.gender?.errors?.[0]
            });
            return;
        }
        setProfileError(undefined);
        finish(profileData.birthDate!, profileData.alias!, profileData.location!, profileData.gender!);
    }

    return (
        <BasicProfileForm
            mode="register"
            className={`${hidden ? "hidden" : ""} form`}
            data={profileData}
            onChange={setProfileData}
            error={profileError}
            config={{
                alias: true,
                birthDate: true,
                gender: true,
                location: true
            }}
        >
            <Button intent="navigate" onClick={onNext}>next</Button>
        </BasicProfileForm>
    )
}

export type ImageStep = {
    finish: (imageData: ImageData[]) => void
    hidden?: boolean;
}
export function ImageStep({ hidden, finish }: ImageStep) {

    const [data, setData] = useState({ images: [] } as ImageGalleryInputData<ImageData>);
    const [error, setError] = useState({} as ImageGalleryInputError);

    function onFinish() {

        const validationSchema = z.object({
            images: z.array(z.object()).min(1, { error: "you must add at least 1 image" }).max(10, { error: "you can add at most 10 images" })
        });

        const result = validationSchema.safeParse(data);
        if (!result.success) {

            const properties = z.treeifyError(result.error).properties;
            console.log(properties);
            setError({
                images: properties?.images?.errors?.[0]
            });
            return;

        } else {

            finish(data.images);

        }

    }

    return (
        <div className={`w-full flex flex-col gap-3 ${hidden ? "hidden" : ""}`}>
            <ImageGalleryInput<ImageData>
                className="w-full"
                data={data}
                error={error}
                onChange={(images: (ImageData | Image)[]) => {
                    setData({
                        images: images.map((i, index) => ({
                            ...i,
                            order: index
                        }))
                    })
                }}
            />
            <Button intent="save" className="w-full" onClick={onFinish}>finish</Button>
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
        birthDate?: string;
        alias?: string;
        location?: Location;
        gender?: Gender
    }>({});

    const [emailError, setEmailError] = useState<string | undefined>(undefined);

    const steps: Step[] = [
        "email",
        "password",
        "profile",
        "images"
    ]

    async function navigate(step: Step) {
        setStep(step);
    }

    async function finish(images: ImageData[]) {

        const imageWriteDtos = (await Promise.all(images.map(async i => ({
            ...i,
            base64EncodedImageData: await getBase64EncodedData(i.url!)
        })))).map(i => ({
            base64EncodedImageData: i.base64EncodedImageData!,
            mimeType: i.mimeType!,
            order: i.order!,
            cropOffsetX: i.crop?.x!,
            cropOffsetY: i.crop?.y!,
            width: i.crop?.width!,
            height: i.crop?.height!
        }))

        const response = await register({
            email: data.email!,
            alias: data.alias!,
            password: data.password!,
            birthDate: data.birthDate!,
            gender: data.gender!,
            location: data.location,
            images: imageWriteDtos
        });

        if (response == undefined) {

            router.refresh();
            router.push(Routes.protected.search);

        } else {

            const hasEmailAlreadyInUseError = response?.some(e => e.code === "auth.duplicate_username") ?? false;
            if (hasEmailAlreadyInUseError) {
                setStep("email");
                setEmailError("an account with that e-mail already exists");
            }

        }

    }

    return (
        <Surface className={`form flex flex-col gap-5 p-0 ${classNames} w-600px max-w-full`} variant="secondary">
            {step !== "account exists" && <ProgressBar steps={steps} progress={steps.indexOf(step) + 1} onClick={(index) => { navigate(steps[index]) }} />}
            {step === "account exists" && <span className="mx-auto label">{step}</span>}
            <EmailStep
                errorMessage={emailError}
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
                next={(birthDate: string, alias: string, location: Location | undefined, gender: Gender) => {
                    setData(prev => ({
                        ...prev,
                        birthDate,
                        alias,
                        location,
                        gender
                    }))
                    navigate("images");
                }}
            />
            <ImageStep finish={finish} hidden={step !== "images"} />
            <AccountExistsStep
                hidden={step !== "account exists"}
            />
        </Surface>
    )
}