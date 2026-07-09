import TextAreaInput from "@/src/shared/components/inputs/text-area-input/TextAreaInput";
import { MessagePermission } from "@/src/shared/types/domain/aggregates";
import { ReadDto } from "@/src/shared/types/dtos/types";
import { Send, Save, Ban } from "lucide-react";
import Message from "./Message";
import { Message as MessageTypeDef } from "@/src/shared/types/domain/aggregates";

// Caller must guarantee that whenever mode is "reply", replyToMessage is not undefined.
export type MessageCrafterProps = {

    saveMessage: () => void;
    onChange: (value: string) => void;
    onCancel: () => void;
    content: string | undefined,
    replyToMessage: ReadDto<MessageTypeDef, MessagePermission> | undefined,
    mode: "create" | "edit" | "reply";

}
export function MessageCrafter({
    saveMessage,
    content = undefined,
    replyToMessage = undefined,
    mode = "create",
    onChange,
    onCancel,
}: MessageCrafterProps) {

    return (
        <div className="relative flex gap-0">
            {
                mode === "reply" && <Message className="surface-secondary absolute bottom-full mb-1" dto={replyToMessage!} isOwn={false} />
            }
            <TextAreaInput
                initialValue={content}
                onBlur={onChange}
                placeholder="write a message.."
                className="surface-secondary w-full p-3 rounded-l-lg grow-1"
            >
            </TextAreaInput>
            <div className="surface-secondary rounded-r-lg flex flex-col justify-center pr-3 pt-2 pb-2">
                {
                    mode === "create" &&
                    <button className="button button-save max-w-max max-h-max my-auto" onClick={saveMessage}>
                        <Send />
                    </button>
                }
                {
                    (mode === "edit" || mode === "reply") &&
                    <div className="flex flex-col gap-2 justify-center">
                        <button className="button button-save" onClick={saveMessage}>{mode === "edit" ? <Save /> : <Send />}</button>
                        <button className="button button-cancel" onClick={onCancel}><Ban /></button>
                    </div>
                }
            </div>
        </div>
    );

}