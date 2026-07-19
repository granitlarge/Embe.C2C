'use client'

import { ReactNode, createContext, useRef, useContext } from 'react'
import { useStore } from 'zustand'
import { ApplicationStore, createApplicationStore } from './store'
import { User, UserPermission } from '../types/domain/aggregates'
import { ReadDto } from '../types/dtos/types'
import { Notification } from '../types/domain/aggregates'

export type ApplicationStoreApi = ReturnType<typeof createApplicationStore>

export const ApplicationStoreContext = createContext<ApplicationStoreApi | undefined>(
  undefined,
)

export interface ApplicationStoreProviderProps {
  user?: ReadDto<User, UserPermission>
  notifications?: ReadDto<Notification, NotificationPermission>[]
  children: ReactNode;
}

export const ApplicationStoreProvider = ({
  user,
  notifications,
  children,
}: ApplicationStoreProviderProps) => {
  const storeRef = useRef<ApplicationStoreApi>(null)
  if (!storeRef.current) {
    storeRef.current = createApplicationStore({
      user: user,
      notifications: notifications ?? []
    })
  }

  return (
    <ApplicationStoreContext.Provider value={storeRef.current}>
      {children}
    </ApplicationStoreContext.Provider>
  )
}

export const useApplicationStore = <T,>(
  selector: (store: ApplicationStore) => T,
): T => {
  const applicationStoreContext = useContext(ApplicationStoreContext)
  if (!applicationStoreContext) {
    throw new Error(`useApplicationStore must be used within ApplicationStoreProvider`)
  }

  return useStore(applicationStoreContext, selector)
}