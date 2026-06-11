// A set is a collection of entities: <Set>
// Each entity has a unique ID within its set : <Set>:<ID>
// An entity may own collections of other entities. <Set>:<ID>:<Set>
export type Guid = string & { readonly __brand: unique symbol };
export const NullGuid: Guid = "00000000-0000-0000-0000-000000000000" as Guid;

export type Set =
    "user" |
    "account" |
    "blocking" |
    "contact" |
    "contact-request" |
    "judgement" |
    "matching" |
    "message" |
    "transaction" |
    "notification";

export type Entity = `${Set}:${Guid}`;
export type Collection = `${Set}:${Guid}:${Set}`;
export type Tag =
    Entity |
    Collection;