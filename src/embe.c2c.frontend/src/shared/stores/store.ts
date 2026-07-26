import { createStore } from 'zustand/vanilla'
import { Matching, MatchingPermission, User, UserPermission } from '../types/domain/aggregates'
import { ReadDto } from '../types/dtos/types'
import { Notification } from '../types/domain/aggregates'

export type ApplicationState = {
    user: ReadDto<User, UserPermission> | undefined;
    notifications: ReadDto<Notification, NotificationPermission>[];
    matchings: ReadDto<Matching, MatchingPermission>[];
}

export type ApplicationActions = {
    setUser: (newUser: ReadDto<User, UserPermission> | undefined) => void;
    setNotifications: (newNotifications: ReadDto<Notification, NotificationPermission>[]) => void;
    setMatchings: (newMatchings: ReadDto<Matching, MatchingPermission>[]) => void;
}

export type ApplicationStore = ApplicationState & ApplicationActions
export const defaultInitState: ApplicationState = { user: undefined, notifications: [], matchings: [] }

export const createApplicationStore = (initState: ApplicationState = defaultInitState) => {
    return createStore<ApplicationStore>()((set) => ({
        ...initState,
        setUser: (newUser: ReadDto<User, UserPermission> | undefined) => set((prev) => ({ ...prev, user: newUser })),
        setNotifications: (newNotifications: ReadDto<Notification, NotificationPermission>[]) => set((prev) => ({
            ...prev,
            notifications: newNotifications
        })),
        setMatchings: (newMatchings: ReadDto<Matching, MatchingPermission>[]) => set((prev) => ({
            ...prev,
            matchings: newMatchings
        })),
    }))
}
