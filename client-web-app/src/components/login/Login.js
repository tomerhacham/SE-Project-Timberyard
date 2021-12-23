import React, { useState, useEffect } from 'react';
import { Avatar, Button, CssBaseline, TextField, Paper, Box, Grid, Typography  } from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import Background_Image from '../../logs_background.jpeg';

const Login = () => {
    const navigate = useNavigate();
    const { login, isLoggedIn } = useAuth();
    const [userInput, setUserInput] = useState({
        email: '',
        password: ''
    })
    const [errorMessage, setErrorMessage] = useState('');
    const [loginError, setLoginError] = useState(false);
    // TODO: Implement correctly, change to false, fix width...
    const [adminWindow, setAdminWindow] = useState(true);
    
    const handleLogin = async (event) => {
        event.preventDefault();

        console.log(userInput);
        
        login(userInput.email, userInput.password, setErrorMessage);
        navigate('/');
    };

    useEffect(() => {
        if (errorMessage !== '') {
            setLoginError(true);
        }
    }, [errorMessage]);

    useEffect(() => {
        if (isLoggedIn) {
            navigate('/');
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [isLoggedIn]);

    return (
        <Grid container component="main" sx={{ height: '100vh' }}>
            <CssBaseline />
            <Grid item xs={false} sm={4} md={7}
                sx={{
                    backgroundImage: `url(${Background_Image})`,
                    backgroundRepeat: 'no-repeat',
                    backgroundColor: (t) =>
                    t.palette.mode === 'light' ? t.palette.grey[50] : t.palette.grey[900],
                    backgroundSize: 'cover',
                    backgroundPosition: 'center',
                }}
            />
            <Grid item xs={12} sm={8} md={5} component={Paper} elevation={6} square>
                <Box
                    sx={{
                        my: 8,
                        mx: 4,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                    }}
                >
                    <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                        <LockOutlinedIcon />
                    </Avatar>
                    <Typography component="h1" variant="h5">
                        Sign in
                    </Typography>
                    {/* TODO: Change to proper error */}
                    {loginError && (
                        <Typography>Invalid email address or password.</Typography>
                    )}
                    <Box component="form" noValidate onSubmit={handleLogin} sx={{ mt: 1 }}>
                        <TextField
                            id="login-user-email"
                            name="email"
                            margin="normal"
                            required
                            fullWidth
                            label="Email Address"
                            type="email"
                            autoComplete="email"
                            autoFocus
                            onChange={(e) => setUserInput({ ...userInput, email: e.target.value })}
                            error={loginError}
                        />
                        {adminWindow && (
                            <TextField
                            id="login-admin-password"
                            name="password"
                            margin="normal"
                            required
                            fullWidth
                            label="Password"
                            type="password"
                            autoComplete="current-password"
                            onChange={(e) => setUserInput({ ...userInput, password: e.target.value })}
                            error={loginError}
                            />
                        )}
                        {/* <FormControlLabel
                            control={<Checkbox value="remember" color="primary" />}
                            label="Remember me"
                        /> */}
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            disabled={userInput.email === '' || userInput.password === ''}
                            sx={{ mt: 3, mb: 2 }}
                        >
                            Sign In
                        </Button>
                        <Grid container>
                            <Grid item xs>
                                <Button variant="body2" onClick={(e) => {
                                    e.preventDefault();  
                                    setAdminWindow(!adminWindow)
                                }}>
                                    {adminWindow ? "I'm a User" : "I'm an Admin"}
                                </Button>
                            </Grid>
                        </Grid>
                    </Box>
                </Box>
            </Grid>
        </Grid>
    )
}

export default Login;
