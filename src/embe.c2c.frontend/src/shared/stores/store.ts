import { createStore } from 'zustand/vanilla'
import { Candidate, CandidatePermission, Matching, MatchingPermission, Message, MessagePermission, User, UserPermission } from '../types/domain/aggregates'
import { ReadDto } from '../types/dtos/types'
import { Notification } from '../types/domain/aggregates'

function sortMessages(messages: ReadDto<Message, MessagePermission>[]) {
    return messages.sort((a, b) => new Date(a.data.createdAt ?? new Date().toISOString()).getTime() - new Date(b.data.createdAt ?? new Date().toISOString()).getTime());
}

export function prepareMatchings(matchings: ReadDto<Matching, MatchingPermission>[]) {
    matchings.sort((a, b) => new Date(a.data.createdAt ?? new Date().toISOString()).getTime() - new Date(b.data.createdAt ?? new Date().toISOString()).getTime());
    matchings.forEach(m => sortMessages(m.data.messages ?? []));
    return matchings.map(m => {
        return {
            ...m,
            data: {
                ...m.data,
                lastMessage: (m.data.messages?.length ?? 0) > 0 ? m.data.messages![m.data.messages!.length - 1] : undefined
            }
        }
    })
}

export function preparePositiveJudgements(candidates: ReadDto<Candidate, CandidatePermission>[]) {
    candidates.sort((a, b) => new Date(a.data.updatedAt!).getTime() - new Date(b.data.updatedAt!).getTime());
    return candidates;
}

export function prepareNotifications(notifications: ReadDto<Notification, NotificationPermission>[]) {
    return notifications;
}

export type Updater<T> = (prev: T) => T;
export type SetState<T> = (updater: Updater<T>) => void

export type ApplicationState = {
    user: ReadDto<User, UserPermission> | undefined;
    notifications: ReadDto<Notification, NotificationPermission>[];
    matchings: ReadDto<Matching, MatchingPermission>[];
    positiveJudgements: ReadDto<Candidate, CandidatePermission>[];
}

export type ApplicationActions = {
    setUser: SetState<ReadDto<User, UserPermission> | undefined>;
    setNotifications: SetState<ReadDto<Notification, NotificationPermission>[]>;
    setMatchings: SetState<ReadDto<Matching, MatchingPermission>[]>;
    setPositiveJudgements: SetState<ReadDto<Candidate, CandidatePermission>[]>
}

export type ApplicationStore = ApplicationState & ApplicationActions
export const defaultInitState: ApplicationState = { user: undefined, notifications: [], matchings: [], positiveJudgements: [] }

export const createApplicationStore = (initState: ApplicationState = defaultInitState) => {
    return createStore<ApplicationStore>()((set) => ({
        ...initState,
        setUser: (updater: Updater<ReadDto<User, UserPermission> | undefined>) => set((prev) => ({ ...prev, user: updater(prev.user) })),
        setNotifications: (updater: Updater<ReadDto<Notification, NotificationPermission>[]>) => set((prev) => ({
            ...prev,
            notifications: prepareNotifications(updater(prev.notifications))
        })),
        setMatchings: (updater: Updater<ReadDto<Matching, MatchingPermission>[]>) => set((prev) => ({
            ...prev,
            matchings: prepareMatchings(updater(prev.matchings))
        })),
        setPositiveJudgements: (updater: Updater<ReadDto<Candidate, CandidatePermission>[]>) => set((prev) => ({
            ...prev,
            positiveJudgements: preparePositiveJudgements(updater(prev.positiveJudgements))
        }))
    }))
}