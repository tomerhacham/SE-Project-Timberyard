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
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { RequestVerificationCode } from '../../api/Api';
import Background_Image from '../../logs_background.jpeg';
import { ROLE } from '../../constants/constants';

const Login = () => {
    const navigate = useNavigate();
    const { loginAction, isLoggedIn } = useAuth();
    const [userInput, setUserInput] = useState({
        email: '',
        password: '',
    });
    // TODO: Handle error
    const [errorMessage, setErrorMessage] = useState('');
    const [loginError, setLoginError] = useState(false);
    const [adminWindow, setAdminWindow] = useState(true);
    const [passwordSent, setPasswordSent] = useState(false);
    const [message, setMessage] = useState(null);

    const handleLogin = async (event) => {
        event.preventDefault();

        console.log(userInput);
        if (!passwordSent) {
            const response = await RequestVerificationCode({
                email: userInput.email,
            });
            if (response) {
                setPasswordSent(true);
                setMessage({
                    text: 'If a user exists with this email address, a verification code will be sent to it.',
                    severity: 'info',
                });
            }
        } else {
            const role = adminWindow ? ROLE.ADMIN : ROLE.USER;
            loginAction(userInput.email, userInput.password, role);
        }
        // navigate('/');
    };

    const disableSignInButton = () => {
        if (adminWindow || passwordSent) {
            return userInput.email === '' || userInput.password === '';
        }
        return userInput.email === '';
    };

    useEffect(() => {
        if (errorMessage !== '') {
            setLoginError(true);
        }
    }, [errorMessage]);

    useEffect(() => {
        setPasswordSent(false);
    }, [adminWindow]);

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
                    {/* TODO: Change to proper error */}
                    {message && (
                        <Message
                            text={message.text}
                            severity={message.severity}
                        />
                    )}
                    {loginError && <Typography>{errorMessage}</Typography>}
                    <Box
                        component='form'
                        onSubmit={handleLogin}
                        sx={{ mt: 1, width: '100%' }}>
                        <TextField
                            id='login-user-email'
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
                            error={loginError}
                        />
                        {(adminWindow || passwordSent) && (
                            <TextField
                                id='login-admin-password'
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
                                error={loginError}
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
                            disabled={disableSignInButton()}
                            sx={{ mt: 3, mb: 2 }}>
                            Sign In
                        </Button>
                        <Grid container>
                            <Grid item xs>
                                <Button
                                    variant='body2'
                                    onClick={(e) => {
                                        e.preventDefault();
                                        setAdminWindow(!adminWindow);
                                    }}>
                                    {adminWindow
                                        ? "I'm a User"
                                        : "I'm an Admin"}
                                </Button>
                            </Grid>
                        </Grid>
                    </Box>
                </Box>
            </Grid>
        </Grid>
    );
};

export default Login;
