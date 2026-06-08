import { useState } from "react";
import HorizontalEllipsis from "../icons/HorizontalEllipsis";

export type MenuProps = {
    className?: string;
    dropDownClassName?: string;
    buttonsClassName?: string;
    items: MenuItem[];
}

export type MenuItem = {
    label: string;
    onClick: () => void;
}

export default function Menu({ className, dropDownClassName, buttonsClassName, items }: MenuProps) {

    const [hidden, setHidden] = useState(true);
    const classNames = [
        className
    ].filter(Boolean).join(" ");
    const dropDownClassNames = [
        dropDownClassName
    ].filter(Boolean).join(" ");
    const buttonsClassNames = [
        buttonsClassName
    ].filter(Boolean).join(" ");

    return (
        <div className={`relative cursor-pointer ${classNames}`} onClick={() => setHidden(prev => !prev)} aria-haspopup="true" aria-expanded={!hidden}>
            <HorizontalEllipsis className="size-10" />
            {
                !hidden && <ul
                    className=
                    {`
                            absolute 
                            -right-10
                            -mt-1
                            w-48 
                            rounded-md 
                            shadow-lg 
                            py-1 
                            z-20 
                            ${dropDownClassNames}
                   `}
                    role="menu"
                    aria-orientation="vertical"
                    aria-labelledby="options-menu">
                    {items.map((item, index) => (
                        <button key={index} onClick={(e) => {
                            e.stopPropagation();
                            item.onClick();
                            setHidden(true);
                        }} className={`text-right ${buttonsClassNames}`} role="menuitem">
                            {item.label}
                        </button>
                    ))}
                </ul>
            }
        </div>
    );

}