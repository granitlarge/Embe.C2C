import * as NextLink from "next/link"

export type LinkProps = {
    className?: string;
    children?: React.ReactNode;
    href: string;
    title?: string;
}
export default function Link({ title, href, className, children, ...props }: LinkProps) {

    const classNames = [
        className
    ].filter(Boolean).join(" ");

    return (
        <NextLink.default href={href} className={`${classNames} active:scale-95`} title={title} {...props}>
            {children}
        </NextLink.default>
    )

}