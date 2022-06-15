// Authentication
export const LOGIN_API = '**/Authentication/Login';
export const LOGIN_ALIAS = 'loginPost';
export const UNSUCCESSFUL_LOGIN_MESSAGE = 'Incorrect email address or password';
export const REQUEST_PASSWORD_API = '**/Authentication/RequestVerificationCode';
export const REQUEST_PASSWORD_ALIAS = 'reqPassword';
export const FORGOT_PASSWORD_API = '**/Authentication/ForgetPassword';
export const FORGOT_PASSWORD_ALIAS = 'forgotPassword';
export const ADD_USER_API = '**/Authentication/AddUser';
export const ADD_USER_ALIAS = 'addUser';
export const REMOVE_USER_API = '**/Authentication/RemoveUser';
export const REMOVE_USER_ALIAS = 'removeUser';
export const ADD_SYSTEM_ADMIN_API = '**/Authentication/AddSystemAdmin';
export const ADD_SYSTEM_ADMIN_ALIAS = 'addSystemAdmin';
export const CHANGE_PASSWORD_API =
    '**/Authentication/ChangeSystemAdminPassword';
export const CHANGE_PASSWORD_ALIAS = 'changePassword';

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
export const GET_ALL_ALARMS_ALIAS = 'getAllAlarms';
export const ADD_ALARM_API = '**/Alarms/AddNewAlarm';
export const ADD_ALARM_ALIAS = 'addNewAlarm';
export const EDIT_ALARM_API = '**/Alarms/EditAlarm';
export const EDIT_ALARM_ALIAS = 'editAlarm';
export const REMOVE_ALARM_API = '**/Alarms/RemoveAlarm';
export const REMOVE_ALARM_ALIAS = 'removeAlarm';

export const SUCCESS_CODE = 200;
