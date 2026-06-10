export type Padding = "none" | "sm" | "md" | "lg";
export type SurfaceProps = {
    className?: string;
    children: React.ReactNode;
    padding?: Padding;
    as?: React.ElementType;
};

export default function Surface({ className, children, padding = "md", as: Component = "div" }: SurfaceProps) {
    const classNames = [
        className,
        padding === "none" ? "p-0" : padding === "sm" ? "p-2" : padding === "md" ? "p-4" : "p-6"
    ].filter(Boolean).join(" ");
    return (
        <Component className={`${classNames} bg-(--surface) text-(--surface-font-color) rounded-md`}>
            {children}
        </Component>
    );
}