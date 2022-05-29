import React from 'react';
import { Box, Container, Typography } from '@mui/material';
import SettingsPassword from './SettingsPassword';
import Alarms from './alarms/Alarms';
import AlarmsTable from './alarms/AlarmsTable';

const Settings = () => {
    return (
        <Box
            component='main'
            sx={{
                flexGrow: 1,
                py: 8,
            }}>
            <Container maxWidth='lg'>
                <Typography sx={{ mb: 3 }} variant='h4'>
                    Settings
                </Typography>
                <Alarms />
                <Box sx={{ pt: 3 }}>
                    <SettingsPassword />
                </Box>
            </Container>
        </Box>
    );
};

export default Settings;
