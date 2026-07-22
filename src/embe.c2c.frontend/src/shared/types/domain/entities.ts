import { Guid } from "../../cache";
import { ImageDetails as ImageDetails } from "./value-objects";

export type Image = {
    id: Guid;
    ownerUserId: Guid;
    imageDetails: ImageDetails;
    markedForDeletionAt: string | null;
    deletedAt: string | null;
    createdAt: string;
}