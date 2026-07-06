import { create } from "zustand"
import { User, UserPermission } from "../types/domain/aggregates"
import { ReadDto } from "../types/dtos/types";

export type CurrentUserStore = {
    currentUser: ReadDto<User, UserPermission> | undefined,
    setCurrentUser: (currentUser: ReadDto<User, UserPermission> | undefined) => void
}
const useCurrentUserStore = create<CurrentUserStore>((set) => ({
    currentUser: undefined,
    setCurrentUser: (currentUser: ReadDto<User, UserPermission> | undefined) => set({ currentUser })
}));

export default useCurrentUserStore