import React, { useState } from 'react';
import { omit } from 'lodash';
import {
    Box,
    Button,
    Card,
    CardContent,
    CardHeader,
    Divider,
    TextField,
    MenuItem,
    Stack,
} from '@mui/material';
import Message from '../../../generic-components/Message';
import { ManageUser } from '../../../api/Api';
import {
    ADD_USER_URL,
    ADD_ADMIN_URL,
    REMOVE_USER_URL,
    MESSAGE,
    ROLE,
} from '../../../constants/constants';

const UsersSettings = (props) => {
    const [email, setEmail] = useState('');
    const [managedRole, setManagedRole] = useState(ROLE.USER);
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
                text: result?.message,
                severity:
                    result?.status || result ? MESSAGE.SUCCESS : MESSAGE.ERROR,
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
                    <Stack direction='row' spacing={2}>
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
                        <TextField
                            style={{
                                width: '20%',
                                marginTop: '16px',
                                marginBottom: '8px',
                            }}
                            id='role-select'
                            label='Role'
                            required
                            value={managedRole}
                            select
                            onChange={(e) => setManagedRole(e.target.value)}>
                            {Object.keys(omit(ROLE, 'UNAUTHORIZE')).map(
                                (code) => (
                                    <MenuItem
                                        key={`menu-item-role-${ROLE[code]}`}
                                        value={ROLE[code]}>
                                        {ROLE[code]}
                                    </MenuItem>
                                )
                            )}
                        </TextField>
                    </Stack>
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
                        onClick={() =>
                            handleSubmit(
                                managedRole === ROLE.ADMIN
                                    ? ADD_ADMIN_URL
                                    : ADD_USER_URL
                            )
                        }
                        style={{ margin: 10 }}>
                        {managedRole === ROLE.ADMIN ? 'Add Admin' : 'Add User'}
                    </Button>
                    <Button
                        color='primary'
                        variant='contained'
                        disabled={email === ''}
                        onClick={() => handleSubmit(REMOVE_USER_URL)}
                        style={{ margin: 10 }}>
                        Remove Account
                    </Button>
                </Box>
            </Card>
        </form>
    );
};

export default UsersSettings;
