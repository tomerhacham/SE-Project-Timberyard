import React from 'react';
import { Box, Container, Grid, Typography } from '@mui/material';
import logo from '../../main_logo.png';
import '../../App.css';

const DashboardApp = () => {
    return (
        <Box component='main' sx={{ flexGrow: 1, py: 8 }}>
            <Container maxWidth={false}>
                <Grid
                    container
                    spacing={0}
                    direction='column'
                    alignItems='center'
                    justifyContent='center'>
                    <Grid item>
                        <img src={logo} className='App-logo' alt='logo' />
                        <Typography
                            component='h1'
                            variant='h5'
                            sx={{
                                textAlign: 'center',
                                letterSpacing: 6,
                                fontSize: 28,
                            }}>
                            Timber Yard
                        </Typography>
                    </Grid>
                </Grid>
            </Container>
        </Box>
    );
};

export default DashboardApp;
