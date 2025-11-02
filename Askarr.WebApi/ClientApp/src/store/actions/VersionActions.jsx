export const GET_VERSION_INFO = "version:get_version_info";

export function getVersionInfo() {
  return (dispatch) => {
    return fetch("/api/version", {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      }
    })
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch version info');
        }
        return response.json();
      })
      .then(data => {
        dispatch({
          type: GET_VERSION_INFO,
          payload: data
        });
        return { ok: true, data: data };
      })
      .catch(error => {
        console.error('Error fetching version info:', error);
        return { 
          ok: false, 
          error: error.message,
          data: {
            currentVersion: '1.0.0',
            latestVersion: '1.0.0',
            updateAvailable: false,
            downloadUrl: 'https://github.com/AmazingMoaaz/Askarr/releases/latest',
            githubUrl: 'https://github.com/AmazingMoaaz/Askarr',
            releasesUrl: 'https://github.com/AmazingMoaaz/Askarr/releases',
            wikiUrl: 'https://github.com/AmazingMoaaz/Askarr/wiki',
            issuesUrl: 'https://github.com/AmazingMoaaz/Askarr/issues'
          }
        };
      });
  };
}

