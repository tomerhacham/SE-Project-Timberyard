import React, { Fragment, useState } from 'react';
import {
    Box,
    Container,
    Typography,
    Accordion,
    AccordionDetails,
    AccordionSummary,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { useAuth } from '../../contexts/AuthContext';
import PasswordSettings from './PasswordSettings';
import Alarms from './alarms/Alarms';
import { ROLE } from '../../constants/constants';
import UsersSettings from './users/UsersSettings';

// TODO: Check if this page should be rendered for regular user also

const Settings = () => {
    const { accessToken } = useAuth();
    const { role } = accessToken;
    const [expanded, setExpanded] = useState(false);

    const handleChange = (panel) => (event, isExpanded) => {
        setExpanded(isExpanded ? panel : false);
    };

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
                {role === ROLE.ADMIN && (
                    <Fragment>
                        <Accordion
                            expanded={expanded === 'alarms'}
                            onChange={handleChange('alarms')}>
                            <AccordionSummary
                                expandIcon={
                                    <ExpandMoreIcon id='alarms-settings-expand' />
                                }
                                aria-controls='alarms-content'
                                id='alarms-header'>
                                <Typography
                                    sx={{ width: '33%', flexShrink: 0 }}>
                                    Alarms Settings
                                </Typography>
                            </AccordionSummary>
                            <AccordionDetails>
                                <Alarms />
                            </AccordionDetails>
                        </Accordion>
                        <Accordion
                            expanded={expanded === 'password'}
                            onChange={handleChange('password')}>
                            <AccordionSummary
                                expandIcon={
                                    <ExpandMoreIcon id='password-settings-expand' />
                                }
                                aria-controls='password-content'
                                id='password-header'>
                                <Typography
                                    sx={{ width: '33%', flexShrink: 0 }}>
                                    Password Settings
                                </Typography>
                            </AccordionSummary>
                            <AccordionDetails>
                                <Box sx={{ pt: 3 }}>
                                    <PasswordSettings />
                                </Box>
                            </AccordionDetails>
                        </Accordion>
                        <Accordion
                            expanded={expanded === 'users'}
                            onChange={handleChange('users')}>
                            <AccordionSummary
                                expandIcon={
                                    <ExpandMoreIcon id='users-settings-expand' />
                                }
                                aria-controls='users-content'
                                id='users-header'>
                                <Typography
                                    sx={{ width: '33%', flexShrink: 0 }}>
                                    Users Settings
                                </Typography>
                            </AccordionSummary>
                            <AccordionDetails>
                                <Box sx={{ pt: 3 }}>
                                    <UsersSettings />
                                </Box>
                            </AccordionDetails>
                        </Accordion>
                    </Fragment>
                )}
            </Container>
        </Box>
    );
};

export default Settings;
