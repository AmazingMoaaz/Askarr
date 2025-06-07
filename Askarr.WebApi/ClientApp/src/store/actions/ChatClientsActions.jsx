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
                "BotToken": settings?.botToken || "",
            })
        })
            .then(response => {
                if (!response.ok) {
                    console.error("Test Discord settings failed with status:", response.status);
                    return response.text().then(text => {
                        try {
                            return { ok: false, error: JSON.parse(text) };
                        } catch (e) {
                            return { ok: false, error: text || `HTTP error! Status: ${response.status}` };
                        }
                    });
                }
                return response.text().then(text => {
                    try {
                        const data = JSON.parse(text);
                        console.log("Discord test response:", data);
                        return { ok: true, data };
                    } catch (e) {
                        console.error("Failed to parse JSON response:", text);
                        return { ok: true }; // Assume success if we got a 200 response
                    }
                });
            })
            .catch(error => {
                console.error("Network error testing Discord settings:", error);
                return { ok: false, error: error.message };
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
                "botToken": settings?.telegramBotToken || "",
            })
        })
            .then(response => {
                if (!response.ok) {
                    console.error("Test Telegram settings failed with status:", response.status);
                    return response.text().then(text => {
                        try {
                            return { ok: false, error: JSON.parse(text) };
                        } catch (e) {
                            return { ok: false, error: text || `HTTP error! Status: ${response.status}` };
                        }
                    });
                }
                return response.text().then(text => {
                    if (!text) {
                        return { ok: true };
                    }
                    
                    try {
                        const data = JSON.parse(text);
                        console.log("Telegram test response:", data);
                        if (data.ok) {
                            return { ok: true, botUsername: data.botUsername };
                        }
                        return { ok: false, error: data.error || "Unknown error" };
                    } catch (e) {
                        console.error("Failed to parse JSON response:", text);
                        // If we get here with a 200 response, assume it worked
                        return { ok: true };
                    }
                });
            })
            .catch(error => {
                console.error("Network error testing Telegram settings:", error);
                return { ok: false, error: error.message };
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
                'UsePrivateResponses': saveModel.usePrivateResponses,
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
            .then(response => {
                if (!response.ok) {
                    console.error("Save settings failed with status:", response.status);
                    return response.text().then(text => {
                        try {
                            return { ok: false, error: JSON.parse(text) };
                        } catch (e) {
                            return { ok: false, error: text || `HTTP error! Status: ${response.status}` };
                        }
                    });
                }
                return response.json().then(data => {
                    if (data.ok) {
                        dispatch(setSettings({
                            client: saveModel.client,
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
                            usePrivateResponses: saveModel.usePrivateResponses,
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
                    return { ok: false, error: data.error || "Unknown error" };
                }).catch(e => {
                    console.error("Failed to parse JSON response:", e);
                    return { ok: true }; // Assume success if we got a 200 response
                });
            })
            .catch(error => {
                console.error("Network error saving settings:", error);
                return { ok: false, error: error.message };
            });
    }
};