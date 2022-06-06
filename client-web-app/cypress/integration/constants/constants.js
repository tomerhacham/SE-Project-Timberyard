// Authentication
export const LOGIN_API = '**/Authentication/Login';
export const LOGIN_ALIAS = 'loginPost';
export const UNSUCCESSFUL_LOGIN_MESSAGE = 'Incorrect email address or password';
export const ADD_USER_API = '**/Authentication/AddUser';
export const ADD_USER_ALIAS = 'addUser';
export const UNSUCCESSFUL_ADD_USER_MESSAGE = 'User already exists';
export const UNSUCCESSFUL_REMOVE_USER_MESSAGE = 'No such user exists';
export const REMOVE_USER_API = '**/Authentication/RemoveUser';
export const REMOVE_USER_ALIAS = 'removeUser';
export const ADD_SYSTEM_ADMIN_API = '**/Authentication/AddSystemAdmin';
export const ADD_SYSTEM_ADMIN_ALIAS = 'addSystemAdmin';

// Queries
export const CARD_YIELD_API = '**/Queries/CardYield';
export const CARD_YIELD_POST = 'cardYieldPost';

export const STATION_YIELD_API = '**/Queries/StationsYield';
export const STATION_YIELD_POST = 'stationYieldPost';

export const STATION_CARD_YIELD_API = '**/Queries/StationAndCardYield';
export const STATION_CARD_YIELD_POST = 'stationCardYieldPost';

export const NFF_API = '**/Queries/NFF';
export const NFF_POST = 'nffPost';

export const TESTER_LOAD_API = '**/Queries/TesterLoad';
export const TESTER_LOAD_POST = 'testerLoadPost';

export const CARD_TEST_DURATION_API = '**/Queries/CardTestDuration';
export const CARD_TEST_DURATION_POST = 'cardTestDurationPost';

export const BOUNDARIES_API = '**/Queries/Boundaries';
export const BOUNDARIES_POST = 'boundariesPost';

// Alarms
export const GET_ALL_ALARMS_API = '**/Alarms/GetAllAlarms';
export const GET_ALL_ALARMS_POST = 'getAllAlarmsPost';

export const SUCCESS_CODE = 200;
