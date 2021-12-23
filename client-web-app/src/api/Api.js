const API_URL = 'https://localhost:5001/api';


export function CardYield(data) {   
    return fetch(`${API_URL}/Queries/CardYield`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
}