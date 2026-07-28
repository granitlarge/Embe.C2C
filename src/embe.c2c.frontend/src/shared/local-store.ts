type LocalStoreData = {
    readonly askedForNotificationPermissions: boolean
}

export class LocalStore {

    private static readonly _localStorageKey: string = "local_store";

    private constructor(data: LocalStoreData) {
        this.data = data;
    }

    public data: LocalStoreData;
    public update(updater: (prev: LocalStoreData) => LocalStoreData) {
        this.data = updater(this.data);
        this.save();
    }

    public static read(): LocalStore {

        if (!window.localStorage) {
            throw new Error("LocalStore can only be used on the client.")
        }

        const item = window.localStorage.getItem(LocalStore._localStorageKey);

        if (!item) {
            return new LocalStore({
                askedForNotificationPermissions: false
            });
        }

        return new LocalStore(JSON.parse(item) as LocalStoreData);

    }

    private save() {

        window.localStorage.setItem(LocalStore._localStorageKey, JSON.stringify(this.data));

    }

}