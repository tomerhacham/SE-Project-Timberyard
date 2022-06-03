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
import Message from '../../generic-components/Message';
import { ChangeAdminPassword } from '../../api/Api';
import { MESSAGE } from '../../constants/constants';

const PasswordSettings = (props) => {
    const [userInput, setUserInput] = useState({
        oldPassword: '',
        newPassword: '',
        confirm: '',
    });
    const [message, setMessage] = useState(null);

    const handleSubmit = async () => {
        if (userInput.newPassword !== userInput.confirm) {
            setMessage({
                text: 'Passwords do not match.',
                severity: MESSAGE.WARNING,
            });
            return;
        }

        const result = await ChangeAdminPassword({
            oldPassword: userInput.oldPassword,
            newPassword: userInput.newPassword,
        });
        if (result) {
            setMessage({
                text: result.message,
                severity: result.status ? MESSAGE.SUCCESS : MESSAGE.ERROR,
            });
        }
    };

    return (
        <form {...props}>
            <Card>
                <CardHeader subheader='Change Password' title='Password' />
                <Divider />
                {message && (
                    <Message text={message.text} severity={message.severity} />
                )}
                <CardContent>
                    <TextField
                        fullWidth
                        label='Old Password'
                        margin='normal'
                        name='old-password'
                        onChange={(e) =>
                            setUserInput({
                                ...userInput,
                                oldPassword: e.target.value,
                            })
                        }
                        type='password'
                        value={userInput.oldPassword}
                        variant='outlined'
                    />
                    <TextField
                        fullWidth
                        label='New Password'
                        margin='normal'
                        name='new-password'
                        onChange={(e) =>
                            setUserInput({
                                ...userInput,
                                newPassword: e.target.value,
                            })
                        }
                        type='password'
                        value={userInput.newPassword}
                        variant='outlined'
                    />
                    <TextField
                        fullWidth
                        label='Confirm password'
                        margin='normal'
                        name='confirm'
                        onChange={(e) =>
                            setUserInput({
                                ...userInput,
                                confirm: e.target.value,
                            })
                        }
                        type='password'
                        value={userInput.confirm}
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
                        onClick={() => handleSubmit()}>
                        Update
                    </Button>
                </Box>
            </Card>
        </form>
    );
};

export default PasswordSettings;
