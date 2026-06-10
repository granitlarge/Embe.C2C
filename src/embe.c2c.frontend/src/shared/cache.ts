// A set is a collection of entities: <Set>
// Each entity has a unique ID within its set : <Set>:<ID>
// An entity may own collections of other entities. <Set>:<ID>:<Set>
export type Id = string;
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

export type Entity = `${Set}:${Id}`;
export type Collection = `${Set}:${Id}:${Set}`;
export type Tag =
    Entity |
    Collection |
    `${Set}:${Id}:${Set}:${Id}:${Set}` |
    `${Set}:${Id}:${Set}:${Id}:${Set}:${Id}:${Set}`;