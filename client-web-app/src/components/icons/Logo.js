import PropTypes from 'prop-types';
import { Box } from '@mui/material';

Logo.propTypes = {
    sx: PropTypes.object,
};

export default function Logo({ sx }) {
    return (
        <Box
            component='img'
            src='/static/logo.svg'
            sx={{ width: 200, height: 80, ...sx }}
        />
    );
}
