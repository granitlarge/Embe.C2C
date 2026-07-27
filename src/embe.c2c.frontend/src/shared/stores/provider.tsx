'use client'

import { ReactNode, createContext, useRef, useContext } from 'react'
import { useStore } from 'zustand'
import { ApplicationStore, createApplicationStore, prepareMatchings, prepareNotifications } from './store'
import { Matching, MatchingPermission, User, UserPermission } from '../types/domain/aggregates'
import { ReadDto } from '../types/dtos/types'
import { Notification } from '../types/domain/aggregates'

export type ApplicationStoreApi = ReturnType<typeof createApplicationStore>

export const ApplicationStoreContext = createContext<ApplicationStoreApi | undefined>(
  undefined,
)

export interface ApplicationStoreProviderProps {
  user?: ReadDto<User, UserPermission>
  notifications?: ReadDto<Notification, NotificationPermission>[]
  matchings?: ReadDto<Matching, MatchingPermission>[]
  children: ReactNode;
}

export const ApplicationStoreProvider = ({
  user,
  notifications,
  matchings, 
  children,
}: ApplicationStoreProviderProps) => {
  const storeRef = useRef<ApplicationStoreApi>(null)
  if (!storeRef.current) {
    storeRef.current = createApplicationStore({
      user: user,
      notifications: prepareNotifications(notifications ?? []),
      matchings: prepareMatchings(matchings ?? [])
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