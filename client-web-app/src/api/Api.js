import axios from 'axios';
import { get } from 'lodash';
import {
    ADD_NEW_ALARM_URL,
    EDIT_ALARM_URL,
    REMOVE_ALARM_URL,
    GET_ALL_ALARMS_URL,
    LOGIN_URL,
    CHANGE_ADMIN_PASSWORD_URL,
    REQUEST_VERIFICATION_CODE_URL,
    FORGET_PASSWORD_URL,
    SUCCESS_CODE,
} from '../constants/constants';

const API_URL = 'https://localhost:5001/api';

const getToken = () => {
    let token = localStorage.getItem('access_token');
    if (token) {
        token = JSON.parse(token);
    }
    return token;
};

const catchError = (error) => {
    console.log('ERROR:', error.code);
    if (error.response) {
        // The request was made and the server responded with a status code
        // that falls out of the range of 2xx
        console.log('Response Status:', error.response.status);
        console.log('Response:', error.response);
    } else if (error.request) {
        // The request was made but no response was received
        // `error.request` is an instance of XMLHttpRequest in the browser and an instance of
        // http.ClientRequest in node.js
        console.log('Request:', error.request);
    } else {
        // Something happened in setting up the request that triggered an Error
        console.log('Message:', error.message);
    }
    console.log('Config:', error.config);
};

// User
export async function Login(data) {
    return await axios
        .post(
            `${API_URL}${LOGIN_URL}`,
            JSON.stringify({ email: data.email, password: data.password }),
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        )
        .then((response) => {
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error in response');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}

export async function RequestVerificationCode(data) {
    return await axios
        .post(
            `${API_URL}${REQUEST_VERIFICATION_CODE_URL}`,
            JSON.stringify(data),
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        )
        .then((response) => {
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error in response');
            }
            return response;
        })
        .catch((error) => {
            catchError(error);
        });
}

export async function ManageUser(config) {
    const token = getToken();

    return await axios
        .post(`${API_URL}${config.url}`, JSON.stringify(config.data), {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: token && `Bearer ${get(token, 'token')}`,
            },
        })
        .then((response) => {
            console.log(response);
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error in response');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}

export async function ChangeAdminPassword(data) {
    const token = getToken();

    return await axios
        .post(`${API_URL}${CHANGE_ADMIN_PASSWORD_URL}`, JSON.stringify(data), {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: token && `Bearer ${get(token, 'token')}`,
            },
        })
        .then((response) => {
            console.log(response);
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error in response');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}

export async function ForgotPassword(data) {
    const token = getToken();

    return await axios
        .post(`${API_URL}${FORGET_PASSWORD_URL}`, JSON.stringify(data), {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: token && `Bearer ${get(token, 'token')}`,
            },
        })
        .then((response) => {
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error in response');
            }
            return response;
        })
        .catch((error) => {
            catchError(error);
        });
}

// Queries
export async function QueryPost(config) {
    const token = getToken();

    return await axios
        .post(`${API_URL}${config.url}`, JSON.stringify(config.data), {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: token && `Bearer ${get(token, 'token')}`,
            },
        })
        .then((response) => {
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Could not fetch query results from the server.');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}

// Alarms
export async function GetAllAlarms() {
    const token = getToken();

    return await axios
        .post(
            `${API_URL}${GET_ALL_ALARMS_URL}`,
            {},
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: token && `Bearer ${get(token, 'token')}`,
                },
            }
        )
        .then((response) => {
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Could not fetch alarms data from the server.');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}

export async function AddNewAlarm(data) {
    const token = getToken();

    return await axios
        .post(`${API_URL}${ADD_NEW_ALARM_URL}`, JSON.stringify(data), {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: token && `Bearer ${get(token, 'token')}`,
            },
        })
        .then((response) => {
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error while adding new alarm');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}

export async function EditAlarm(data) {
    const token = getToken();

    return await axios
        .post(`${API_URL}${EDIT_ALARM_URL}`, JSON.stringify(data), {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: token && `Bearer ${get(token, 'token')}`,
            },
        })
        .then((response) => {
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error while editing alarm');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}

export async function RemoveAlarm(data) {
    const token = getToken();

    return await axios
        .post(`${API_URL}${REMOVE_ALARM_URL}`, JSON.stringify(data), {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: token && `Bearer ${get(token, 'token')}`,
            },
        })
        .then((response) => {
            console.log(response);
            if (!response.status === SUCCESS_CODE) {
                console.log(response);
                throw Error('Error while deleting alarm');
            }
            return response.data;
        })
        .catch((error) => {
            catchError(error);
        });
}
