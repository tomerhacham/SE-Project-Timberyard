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

export async function AlarmsPost(config) {
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
