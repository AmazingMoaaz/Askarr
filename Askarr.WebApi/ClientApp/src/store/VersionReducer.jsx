import { GET_VERSION_INFO } from "./actions/VersionActions";

const initialState = {
  currentVersion: '2.5.5',
  latestVersion: '2.5.5',
  updateAvailable: false,
  downloadUrl: 'https://github.com/AmazingMoaaz/Askarr/releases/latest',
  githubUrl: 'https://github.com/AmazingMoaaz/Askarr',
  releasesUrl: 'https://github.com/AmazingMoaaz/Askarr/releases',
  wikiUrl: 'https://github.com/AmazingMoaaz/Askarr/wiki',
  issuesUrl: 'https://github.com/AmazingMoaaz/Askarr/issues'
};

export default function VersionReducer(state = initialState, action) {
  switch (action.type) {
    case GET_VERSION_INFO:
      return {
        ...state,
        ...action.payload
      };
    default:
      return state;
  }
}

