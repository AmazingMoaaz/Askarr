export const GET_SETTINGS = "chatclients:get_settings";

export function setSettings(settings) {
    return {
        type: GET_SETTINGS,
        payload: settings
    };
};

export function getSettings() {
    return (dispatch, getState) => {
        const state = getState();

        return fetch("../api/chatclients", {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${state.user.token}`
            }
        })
            .then(data => data.json())
            .then(data => {
                return dispatch(setSettings(data));
            });
    };
};

export function testSettings(settings) {
    return (dispatch, getState) => {
        const state = getState();

        return fetch("../api/chatclients/discord/test", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${state.user.token}`
            },
            body: JSON.stringify({
                "BotToken": settings.botToken,
            })
        })
            .then(data => data.json())
            .then(data => {
                if (data.ok) {
                    return { ok: true };
                }

                return { ok: false, error: data }
            });
    };
};

export function testTelegramSettings(settings) {
    return (dispatch, getState) => {
        const state = getState();

        return fetch("../api/chatclients/telegram/test", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${state.user.token}`
            },
            body: JSON.stringify({
                "botToken": settings.botToken,
            })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                return response.text();
            })
            .then(text => {
                let data;
                try {
                    data = JSON.parse(text);
                } catch (error) {
                    throw new Error(`Invalid JSON response: ${text}`);
                }
                
                if (data.ok) {
                    return { ok: true, botUsername: data.botUsername };
                }

                return { ok: false, error: data.error || "Unknown error" }
            });
    };
};

export function save(saveModel) {
    return (dispatch, getState) => {
        const state = getState();

        return fetch("../api/chatclients", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': `Bearer ${state.user.token}`
            },
            body: JSON.stringify({
                'Client': saveModel.client,
                'ClientId': saveModel.clientId,
                'BotToken': saveModel.botToken,
                'StatusMessage': saveModel.statusMessage,
                'MonitoredChannels': saveModel.monitoredChannels,
                'TvShowRoles': saveModel.tvShowRoles,
                'MovieRoles': saveModel.movieRoles,
                'MusicRoles': saveModel.musicRoles,
                'EnableRequestsThroughDirectMessages': saveModel.enableRequestsThroughDirectMessages,
                'AutomaticallyNotifyRequesters': saveModel.automaticallyNotifyRequesters,
                'NotificationMode': saveModel.notificationMode,
                'NotificationChannels': saveModel.notificationChannels,
                'AutomaticallyPurgeCommandMessages': saveModel.automaticallyPurgeCommandMessages,
                'TelegramBotToken': saveModel.telegramBotToken,
                'TelegramMonitoredChats': saveModel.telegramMonitoredChats,
                'TelegramNotificationChats': saveModel.telegramNotificationChats,
                'TelegramMovieRoles': saveModel.telegramMovieRoles,
                'TelegramTvRoles': saveModel.telegramTvRoles,
                'TelegramMusicRoles': saveModel.telegramMusicRoles,
                'TelegramEnableRequestsThroughDirectMessages': saveModel.telegramEnableRequestsThroughDirectMessages,
                'TelegramAutomaticallyNotifyRequesters': saveModel.telegramAutomaticallyNotifyRequesters,
                'TelegramNotificationMode': saveModel.telegramNotificationMode,
                'Language': saveModel.language,
            })
        })
            .then(data => data.json())
            .then(data => {
                if (data.ok) {
                    dispatch(setSettings({
                        chatClient: saveModel.client,
                        clientId: saveModel.clientId,
                        botToken: saveModel.botToken,
                        statusMessage: saveModel.statusMessage,
                        monitoredChannels: saveModel.monitoredChannels,
                        tvShowRoles: saveModel.tvShowRoles,
                        movieRoles: saveModel.movieRoles,
                        musicRoles: saveModel.musicRoles,
                        enableRequestsThroughDirectMessages: saveModel.enableRequestsThroughDirectMessages,
                        automaticallyNotifyRequesters: saveModel.automaticallyNotifyRequesters,
                        notificationMode: saveModel.notificationMode,
                        notificationChannels: saveModel.notificationChannels,
                        automaticallyPurgeCommandMessages: saveModel.automaticallyPurgeCommandMessages,
                        telegramBotToken: saveModel.telegramBotToken,
                        telegramMonitoredChats: saveModel.telegramMonitoredChats,
                        telegramNotificationChats: saveModel.telegramNotificationChats,
                        telegramMovieRoles: saveModel.telegramMovieRoles,
                        telegramTvRoles: saveModel.telegramTvRoles,
                        telegramMusicRoles: saveModel.telegramMusicRoles,
                        telegramEnableRequestsThroughDirectMessages: saveModel.telegramEnableRequestsThroughDirectMessages,
                        telegramAutomaticallyNotifyRequesters: saveModel.telegramAutomaticallyNotifyRequesters,
                        telegramNotificationMode: saveModel.telegramNotificationMode,
                        language: saveModel.language,
                    }));
                    return { ok: true };
                }

                return { ok: false, error: data };
            });
    }
};