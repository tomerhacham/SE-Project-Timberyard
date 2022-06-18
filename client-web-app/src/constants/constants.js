// Paths
export const LOGIN_PATH = '/login';
export const SETTINGS_PATH = '/settings';
export const CARD_YIELD_PATH = '/cardyield';
export const STATION_YIELD_PATH = '/stationyield';
export const STATION_CARD_YIELD_PATH = '/stationcardyield';
export const NFF_PATH = '/nff';
export const BOUNDARIES_PATH = '/boundaries';
export const TESTER_LOAD_PATH = '/testerload';
export const CARD_TEST_DURATION_PATH = '/cardtestduration';

// IDs
export const CARD_YIELD_ID = 'cardYield';
export const STATION_YIELD_ID = 'stationYield';
export const BOUNDARIES_ID = 'boundaries';

// Icons
export const CARD_YIELD_ICON = 'SdCardIcon';
export const STATION_YIELD_ICON = 'LocalGasStationIcon';
export const STATION_CARD_YIELD_ICON = 'EvStationIcon';
export const NFF_ICON = 'GppBadIcon';
export const BOUNDARIES_ICON = 'FenceIcon';
export const TESTER_LOAD_ICON = 'TimelapseIcon';
export const CARD_TEST_DURATION_ICON = 'AvTimerIcon';

// Api urls
export const LOGIN_URL = '/Authentication/Login';
export const REQUEST_VERIFICATION_CODE_URL =
    '/Authentication/RequestVerificationCode';
export const ADD_USER_URL = '/Authentication/AddUser';
export const ADD_ADMIN_URL = '/Authentication/AddSystemAdmin';
export const REMOVE_USER_URL = '/Authentication/RemoveUser';
export const CHANGE_ADMIN_PASSWORD_URL =
    '/Authentication/ChangeSystemAdminPassword';
export const FORGET_PASSWORD_URL = '/Authentication/ForgetPassword';
export const GET_ALL_ALARMS_URL = '/Alarms/GetAllAlarms';
export const ADD_NEW_ALARM_URL = '/Alarms/AddNewAlarm';
export const EDIT_ALARM_URL = '/Alarms/EditAlarm';
export const REMOVE_ALARM_URL = '/Alarms/RemoveAlarm';

// Role
export const ROLE = {
    UNAUTHORIZE: 'Unauthorized',
    USER: 'RegularUser',
    ADMIN: 'Admin',
};

// Messages status (severity)
export const MESSAGE = {
    SUCCESS: 'success',
    ERROR: 'error',
    INFO: 'info',
    WARNING: 'warning',
};

// api
export const SUCCESS_CODE = 200;
export const BAD_REQUEST_CODE = 400;
export const UNAUTHORIZED_CODE = 401;
export const ERR_NETWORK_CODE = 'ERR_NETWORK';

// text
export const SEND_VERIFICATION_CODE_TEXT =
    'If a user with this email address exists, a verification code will be sent to it.';
export const FORGOT_PASSWORD_TEXT =
    'Please enter your email address to reset your password';
export const FORGOT_PASSWORD_SENT_TEXT =
    'If a user with this email address exists, an email with a new password will be sent.';
