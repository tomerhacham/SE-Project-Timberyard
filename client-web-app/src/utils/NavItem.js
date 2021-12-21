import { Link, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';
import { Box, Button, ListItem } from '@mui/material';

const NavItem = (props) => {
    const { primary, to, icon, onClick, ...others } = props;
    const location = useLocation();
    const active = to ? (location.pathname === to) : false;

    return (
        <ListItem
        disableGutters
        sx={{
            display: 'flex',
            mb: 0.5,
            py: 0,
            px: 2
        }}
        {...others}
        >
            {/* <Link to={to}> */}
            <Button
            component={Link}
            to={to}
            startIcon={icon}
            onClick={onClick}
            disableRipple
            sx={{
                backgroundColor: active && 'rgba(255,255,255, 0.08)',
                borderRadius: 1,
                color: active ? 'secondary.main' : 'neutral.300',
                fontWeight: active && 'fontWeightBold',
                justifyContent: 'flex-start',
                px: 3,
                textAlign: 'left',
                textTransform: 'none',
                width: '100%',
                '& .MuiButton-startIcon': {
                color: active ? 'secondary.main' : 'neutral.400'
                },
                '&:hover': {
                backgroundColor: 'rgba(255,255,255, 0.08)'
                }
            }}
            >
                <Box sx={{ flexGrow: 1 }}>
                    {primary}
                </Box>
            </Button>
            {/* </Link> */}
        </ListItem>
    )
}

NavItem.propTypes = {
    primary: PropTypes.string,
    to: PropTypes.string,
    icon: PropTypes.node,
    onClick: PropTypes.func || null
};

export default NavItem;