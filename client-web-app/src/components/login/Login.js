import React, { useState, useEffect } from 'react';
import {
    Avatar,
    Button,
    CssBaseline,
    TextField,
    Paper,
    Box,
    Grid,
    Typography,
} from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Message from '../../generic-components/Message';
import Loader from '../../generic-components/Loader';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { ForgotPassword, RequestVerificationCode } from '../../api/Api';
import { validateEmail } from '../../utils/utils';
import Background_Image from '../../logs_background.jpeg';
import {
    SEND_VERIFICATION_CODE_TEXT,
    FORGOT_PASSWORD_TEXT,
    FORGOT_PASSWORD_SENT_TEXT,
    SUCCESS_CODE,
    MESSAGE,
} from '../../constants/constants';

const Login = () => {
    const navigate = useNavigate();
    const { loginAction, isLoggedIn, apiMessage } = useAuth();
    const [userInput, setUserInput] = useState({
        email: '',
        password: '',
    });
    const [havePassword, setHavePassword] = useState(true);
    const [forgotPassword, setForgotPassword] = useState(false);
    const [message, setMessage] = useState(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (forgotPassword) {
            return handleForgotPassword();
        } else if (!havePassword) {
            return handleSendCode();
        }

        // Admin/user with password
        setIsLoading(true);
        await loginAction(userInput.email, userInput.password, setMessage);
        setIsLoading(false);
    };

    const handleSendCode = async () => {
        // Regular user without password
        const response = await RequestVerificationCode({
            email: userInput.email,
        });
        if (response && response.status === SUCCESS_CODE) {
            setMessage({
                text: SEND_VERIFICATION_CODE_TEXT,
                severity: MESSAGE.INFO,
            });
            setHavePassword(true);
        }
    };

    const handleForgotPassword = async () => {
        const result = await ForgotPassword({ email: userInput.email });
        if (result) {
            setMessage({
                text: FORGOT_PASSWORD_SENT_TEXT,
                severity: MESSAGE.INFO,
            });
        }
    };

    const havePasswordButton = (event) => {
        event.preventDefault();
        setMessage(null);

        // If havePwd changing to true, set forgotPwd to false also
        setHavePassword((prevState) => {
            if (!prevState) setForgotPassword(false);
            return !prevState;
        });
    };

    const forgotPasswordButton = () => {
        setForgotPassword(true);
        setHavePassword(false);
        setMessage({
            text: FORGOT_PASSWORD_TEXT,
            severity: MESSAGE.INFO,
        });
    };

    const renderSubmitButtonText = () =>
        havePassword
            ? 'Sign In'
            : forgotPassword
            ? 'Send Reset Email'
            : 'Send Code';

    useEffect(() => {
        if (isLoggedIn) {
            navigate('/');
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [isLoggedIn]);

    return (
        <Grid container component='main' sx={{ height: '100vh' }}>
            <CssBaseline />
            <Grid
                item
                xs={false}
                sm={4}
                md={7}
                sx={{
                    backgroundImage: `url(${Background_Image})`,
                    backgroundRepeat: 'no-repeat',
                    backgroundColor: (t) =>
                        t.palette.mode === 'light'
                            ? t.palette.grey[50]
                            : t.palette.grey[900],
                    backgroundSize: 'cover',
                    backgroundPosition: 'center',
                }}
            />
            <Grid
                item
                xs={12}
                sm={8}
                md={5}
                component={Paper}
                elevation={6}
                square>
                <Box
                    sx={{
                        my: 8,
                        mx: 4,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                    }}>
                    <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                        <LockOutlinedIcon />
                    </Avatar>
                    <Typography component='h1' variant='h5'>
                        Sign in
                    </Typography>
                    {message && !apiMessage && (
                        <Message
                            id='login-page-message'
                            style={{ marginTop: '10px' }}
                            text={message.text}
                            severity={message.severity}
                        />
                    )}
                    <Box
                        component='form'
                        onSubmit={handleSubmit}
                        sx={{ mt: 1, width: '100%' }}>
                        <TextField
                            id='login-email'
                            name='email'
                            margin='normal'
                            required
                            fullWidth
                            label='Email Address'
                            type='email'
                            autoComplete='email'
                            autoFocus
                            onChange={(e) =>
                                setUserInput({
                                    ...userInput,
                                    email: e.target.value,
                                })
                            }
                            error={
                                userInput.email !== '' &&
                                !validateEmail(userInput.email)
                            }
                        />
                        {havePassword && (
                            <TextField
                                id='login-password'
                                name='password'
                                margin='normal'
                                required
                                fullWidth
                                label='Password'
                                type='password'
                                autoComplete='current-password'
                                onChange={(e) =>
                                    setUserInput({
                                        ...userInput,
                                        password: e.target.value,
                                    })
                                }
                            />
                        )}
                        {/* <FormControlLabel
                            control={<Checkbox value="remember" color="primary" />}
                            label="Remember me"
                        /> */}
                        <Button
                            id='login-signIn-button'
                            type='submit'
                            fullWidth
                            variant='contained'
                            disabled={
                                userInput.email === '' ||
                                (havePassword && userInput.password === '')
                            }
                            sx={{ mt: 3, mb: 2 }}>
                            {renderSubmitButtonText()}
                        </Button>
                        <Grid container>
                            <Grid item xs>
                                <Button
                                    id='password-toggle-button'
                                    variant='body2'
                                    onClick={(e) => havePasswordButton(e)}>
                                    {havePassword
                                        ? "I don't have a password"
                                        : 'I have a password'}
                                </Button>
                            </Grid>
                            {havePassword && (
                                <Grid item>
                                    <Button
                                        id='forgot-password-button'
                                        variant='body2'
                                        onClick={forgotPasswordButton}>
                                        Forgot Password?
                                    </Button>
                                </Grid>
                            )}
                        </Grid>
                    </Box>
                    {isLoading && <Loader />}
                </Box>
            </Grid>
        </Grid>
    );
};

export default Login;
