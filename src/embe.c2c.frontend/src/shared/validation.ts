import * as z from 'zod';

export const PasswordValidationRules = z
    .string()
    .min(8, { message: "password must be at least 8 characters long" })
    .refine((value) => /[A-Z]/.test(value), { message: "password must contain at least one uppercase letter" })
    .refine((value) => /[a-z]/.test(value), { message: "password must contain at least one lowercase letter" })
    .refine((value) => /[0-9]/.test(value), { message: "password must contain at least one number" });