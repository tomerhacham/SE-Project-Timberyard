const API_URL = 'https://localhost:5001/api';


export async function CardYield(data) {   
    return await fetch(`${API_URL}/Queries/CardYield`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
}