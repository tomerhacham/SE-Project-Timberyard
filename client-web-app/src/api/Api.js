import {
    ADD_NEW_ALARM_URL,
    EDIT_ALARM_URL,
    REMOVE_ALARM_URL,
} from '../constants/constants';

const API_URL = 'https://localhost:5001/api';

export async function QueryPost(config) {
    return await fetch(`${API_URL}${config.url}`, {
        method: 'POST',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(config.data),
    })
        .then((response) => response.json())
        .then((result) => {
            // TODO: Add functionality? check status etc...
            return result;
        })
        .catch((error) => {
            console.log('Catched error:', error);
        });
}

export async function GetAllAlarms() {
    return await fetch(`${API_URL}/Alarms/GetAllAlarms`, {
        method: 'POST',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
    })
        .then((response) => {
            if (!response.ok) {
                throw Error('Could not fetch alarms data from the server.');
            }
            return response.json();
        })
        .then((result) => {
            return result;
        })
        .catch((err) => {
            console.log('Catched error:', err);
        });
}

export async function AddNewAlarm(data) {
    return await fetch(`${API_URL}${ADD_NEW_ALARM_URL}`, {
        method: 'POST',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
        .then((response) => {
            if (response.ok) {
                return response.json();
            } else {
                return response.statusText;
            }
        })
        .then((result) => {
            return result;
        })
        .catch((error) => {
            console.log('Catched error:', error);
        });
}

export async function EditAlarm(data) {
    return await fetch(`${API_URL}${EDIT_ALARM_URL}`, {
        method: 'POST',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
        .then((response) => {
            if (response.ok) {
                return response.json();
            } else {
                return response.statusText;
            }
        })
        .then((result) => {
            return result;
        })
        .catch((error) => {
            console.log('Catched error:', error);
        });
}

export async function RemoveAlarm(data) {
    return await fetch(`${API_URL}${REMOVE_ALARM_URL}`, {
        method: 'POST',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
        .then((response) => {
            if (response.ok) {
                return response.status;
            } else {
                return response.statusText;
            }
        })
        .then((result) => {
            return result;
        })
        .catch((error) => {
            console.log('Catched error:', error);
        });
}
