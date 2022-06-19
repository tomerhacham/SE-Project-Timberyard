import React, { useState, useEffect } from 'react';
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
    Grid,
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
import UsersTable from './UsersTable';

const UsersSettings = () => {
    const [selectedData, setSelectedData] = useState(undefined);
    const [inputData, setInputData] = useState({ email: '', role: ROLE.USER });
    const [message, setMessage] = useState(null);
    const [updateTable, setUpdateTable] = useState(0);

    const handleSubmit = async (url) => {
        if (url === REMOVE_USER_URL) {
            const confirm = window.confirm(
                'Are you sure you want to remove this user from the system?'
            );
            if (!confirm) {
                return;
            }
        }
        const result = await ManageUser({
            data: { email: inputData.email },
            url,
        });
        if (result) {
            setMessage({
                text: result?.message,
                severity:
                    result?.status || result === true
                        ? MESSAGE.SUCCESS
                        : MESSAGE.ERROR,
            });
            setUpdateTable((prevState) => prevState + 1);
            setSelectedData(undefined);
        } else if (result === false) {
            // AddSystemAdmin returned false
            setMessage({
                text: 'Error in adding admin user',
                severity: MESSAGE.ERROR,
            });
        }
    };

    useEffect(() => {
        if (selectedData) {
            setInputData(selectedData);
        } else {
            setInputData({ email: '', role: ROLE.USER });
        }
    }, [selectedData]);

    return (
        <form>
            <Card>
                <CardHeader subheader='Add or Remove Users' title='Users' />
                <Divider />
                {message && (
                    <Message
                        id='users-settings-message'
                        text={message.text}
                        severity={message.severity}
                    />
                )}
                <CardContent>
                    <Grid item md={12} sm={12} xs={12}>
                        <UsersTable
                            update={updateTable}
                            setData={setSelectedData}
                        />
                    </Grid>
                    <Stack direction='row' spacing={2}>
                        <TextField
                            id='users-settings-email-input'
                            fullWidth
                            label='Email'
                            margin='normal'
                            name='email'
                            value={inputData.email}
                            disabled={selectedData !== undefined}
                            onChange={(e) =>
                                setInputData({
                                    ...inputData,
                                    email: e.target.value,
                                })
                            }
                            type='email'
                            variant='outlined'
                        />
                        <TextField
                            style={{
                                width: '20%',
                                marginTop: '16px',
                                marginBottom: '8px',
                            }}
                            id='users-settings-role-select'
                            label='Role'
                            required
                            value={inputData.role}
                            select
                            disabled={
                                selectedData && selectedData.role === ROLE.ADMIN
                            }
                            onChange={(e) =>
                                setInputData({
                                    ...inputData,
                                    role: e.target.value,
                                })
                            }>
                            {Object.keys(omit(ROLE, 'UNAUTHORIZE')).map(
                                (code, index) => (
                                    <MenuItem
                                        key={index}
                                        id={`menu-item-role-${ROLE[code]}`}
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
                        id='users-settings-add-button'
                        color='primary'
                        variant='contained'
                        disabled={
                            inputData.email === '' ||
                            (selectedData && selectedData.role === ROLE.ADMIN)
                        }
                        onClick={() =>
                            handleSubmit(
                                inputData.role === ROLE.ADMIN
                                    ? ADD_ADMIN_URL
                                    : ADD_USER_URL
                            )
                        }
                        style={{ margin: 10 }}>
                        {inputData.role === ROLE.ADMIN
                            ? 'Add Admin'
                            : 'Add User'}
                    </Button>
                    <Button
                        id='users-settings-remove-button'
                        color='primary'
                        variant='contained'
                        disabled={
                            inputData.email === '' || selectedData === undefined
                        }
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
