import { default as NextLink } from "next/link"
import type { LinkProps as NextLinkProps } from "next/link"

type ExcludedProps = "href" | "children" | "className" | "title";
export type LinkProps = Omit<NextLinkProps, ExcludedProps> & {
    className?: string;
    children?: React.ReactNode;
    href: string;
    title?: string;
}
export default function Link({ title, href, className, children, ...props }: LinkProps) {

    const classNames = [
        className,
    ].filter(Boolean).join(" ");

    return (
        <NextLink href={href} className={`${classNames} active:scale-95`} title={title} {...props}>
            {children}
        </NextLink>
    )

}