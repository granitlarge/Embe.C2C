export const Routes = {
    public: {
        login: "/public/login",
        register: "/public/register",
        forgotPassword: "/public/forgot-password",
        resetPassword: "/public/reset-password"
    },
    protected: {

        discover: "/protected/discover",

        likes: "/protected/likes",

        matches: "/protected/matches",
        match: (matchId: string) => `/protected/matches/${matchId}`,

        me: "/protected/me",

        search: "/protected/search",

        searchProfiles: "/protected/search-profile",
        createSearchProfile: "/protected/search-profile/new",
        searchProfile: (searchProfileId: string) => `/protected/search-profile/${searchProfileId}`,

        settings: "/protected/me/settings",

        user: (userId: string) => `/protected/user/${userId}`

    }
};