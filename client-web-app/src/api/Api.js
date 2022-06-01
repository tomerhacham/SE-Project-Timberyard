import axios from 'axios';
import { get } from 'lodash';
// import { useAuth } from '../contexts/AuthContext';
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

// TODO: Handle errors

// const useApi = () => {
//     const authContextValue = useAuth();

//     const sendRequest = async (config) => {
//         const request = {
//             ...config,
//             method: config.method || 'POST',
//             headers: {
//                 Accept: 'application/json',
//                 'Content-Type': 'application/json',
//             },
//         };
//         let token = localStorage.getItem('token');
//         console.log('TOKEN:', token);

//         if (token) {
//             token = JSON.parse(token);
//             console.log('PARSED TOKEN:', token);
//             request.headers = {
//                 ...request.headers,
//                 Authorization: `Bearer ${get(token, 'token')}`,
//             };
//         }
//         const url = get(request, 'url');
//         return axios(request)
//             .then((response) => {
//                 const status = get('status', response);
//                 if (status !== SUCCESS_CODE) {
//                     console.log('Failed response');
//                 }
//                 return response;
//             })
//             .catch((error) => {
//                 // const errorStatus = get('response.status', error);
//                 console.log('ERROR:', error);
//             });
//     };
//     return { sendRequest };
// };

// export default useApi;

const getToken = () => {
    let token = localStorage.getItem('access_token');
    if (token) {
        token = JSON.parse(token);
    }
    return token;
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
            console.log(response);
            return response.data;
        })
        .catch((error) => {
            console.log('LOGIN ERROR', error);
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
            console.log(response);
            return response;
        })
        .catch((error) => {
            console.log('Request ERROR', error);
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
                throw Error('Response was unsuccessful');
            }
            return response.data;
        })
        .catch((error) => {
            console.log('Catched error in api:', error);
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
                throw Error('Response was unsuccessful');
            }
            return response.data;
        })
        .catch((error) => {
            console.log('Catched error in api:', error);
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
            console.log(response);
            return response.data;
        })
        .catch((error) => {
            console.log('Catched error in api:', error);
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
            console.log('Catched error in api:', error);
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
        .then((result) => {
            return result;
        })
        .catch((err) => {
            console.log('Catched error:', err);
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
            console.log('Catched error in api:', error);
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
            console.log('Catched error in api:', error);
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
                throw Error('Error while deleting alarm');
            }
            return response.data;
        })
        .catch((error) => {
            console.log('Catched error in api:', error);
        });
}
