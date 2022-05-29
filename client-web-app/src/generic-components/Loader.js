import React from 'react';
import { Box, CircularProgress } from '@mui/material';

// TODO: place loader in center of screen
const Loader = () => {
    return (
        <Box sx={{ display: 'flex' }}>
            <CircularProgress id='loader' />
        </Box>
    )
}

export default Loader;