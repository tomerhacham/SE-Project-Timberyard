import React, { useState } from 'react';
import {
    Box,
    Button,
    Card,
    CardContent,
    CardHeader,
    Divider,
    TextField,
} from '@mui/material';
import Message from '../../../generic-components/Message';
import { ManageUser } from '../../../api/Api';
import { ADD_USER_URL, REMOVE_USER_URL } from '../../../constants/constants';

const UsersSettings = (props) => {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState(null);

    const handleChange = (event) => {
        setEmail(event.target.value);
    };

    const handleSubmit = async (url) => {
        if (url === REMOVE_USER_URL) {
            const confirm = window.confirm(
                'Are you sure you want to remove this user from the system?'
            );
            if (!confirm) {
                return;
            }
        }
        const result = await ManageUser({ data: { email }, url });
        if (result) {
            setMessage({
                text: result.message,
                severity: result.status ? 'success' : 'error',
            });
        }
    };

    return (
        <form {...props}>
            <Card>
                <CardHeader subheader='Add or Remove Users' title='Users' />
                <Divider />
                {message && (
                    <Message text={message.text} severity={message.severity} />
                )}
                <CardContent>
                    <TextField
                        fullWidth
                        label='Email'
                        margin='normal'
                        name='email'
                        onChange={handleChange}
                        type='email'
                        value={email}
                        variant='outlined'
                    />
                </CardContent>
                <Divider />
                <Box
                    sx={{
                        display: 'flex',
                        justifyContent: 'flex-end',
                        p: 2,
                    }}>
                    <Button
                        color='primary'
                        variant='contained'
                        disabled={email === ''}
                        onClick={() => handleSubmit(ADD_USER_URL)}
                        style={{ margin: 10 }}>
                        Add User
                    </Button>
                    <Button
                        color='primary'
                        variant='contained'
                        disabled={email === ''}
                        onClick={() => handleSubmit(REMOVE_USER_URL)}
                        style={{ margin: 10 }}>
                        Remove User
                    </Button>
                </Box>
            </Card>
        </form>
    );
};

export default UsersSettings;
