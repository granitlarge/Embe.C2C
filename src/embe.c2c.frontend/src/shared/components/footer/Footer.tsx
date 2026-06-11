import { AuthenticatedUser } from "../../user";
import MainNav from "../nav/MainNav";
import Surface from "../surfaces/Surface";

export type FooterProps = {
    className?: string;
    user?: AuthenticatedUser;
};

export async function Footer({ className, user }: FooterProps) {

    const hasUnseenLikes = true;
    const hasUnseenMatches = true;
    const hasUnseenMessages = true;

    return (
        <Surface as="footer" padding="none" className={className}>
            {
                user && <MainNav
                    user={user}
                    hasUnseenLikes={hasUnseenLikes}
                    hasUnseenMatches={hasUnseenMatches}
                    hasUnseenMessages={hasUnseenMessages}
                /> 
            }
        </Surface>
    )

}