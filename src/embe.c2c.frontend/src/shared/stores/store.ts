import { createStore } from 'zustand/vanilla'
import { User, UserPermission } from '../types/domain/aggregates'
import { ReadDto } from '../types/dtos/types'

export type ApplicationState = {
    user: ReadDto<User, UserPermission> | undefined
}

export type ApplicationActions = {
    setUser: (newUser: ReadDto<User, UserPermission> | undefined) => void
}

export type ApplicationStore = ApplicationState & ApplicationActions
export const defaultInitState: ApplicationState = { user: undefined }

export const createApplicationStore = (initState: ApplicationState = defaultInitState) => {
    return createStore<ApplicationStore>()((set) => ({
        ...initState,
        setUser: (newUser: ReadDto<User, UserPermission> | undefined) => set((_) => ({ user: newUser }))
    }))
}
