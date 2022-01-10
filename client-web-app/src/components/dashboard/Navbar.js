import React from 'react';
import PropTypes from 'prop-types';
import styled from '@emotion/styled';
import { AppBar, Badge, Box, IconButton, Toolbar, Tooltip } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import SearchIcon from '@mui/icons-material/Search';
import NotificationsIcon from '@mui/icons-material/Notifications';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';


const NavbarRoot = styled(AppBar)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    boxShadow: theme.shadows[3]
}));

const Navbar = ({ onSidebarOpen, ...other }) => {

    return (
        <>
            <NavbarRoot
                sx={{
                left: {
                    lg: 280
                },
                width: {
                    lg: 'calc(100% - 280px)'
                }
                }}
                {...other}>
                <Toolbar
                disableGutters
                sx={{
                    minHeight: 64,
                    left: 0,
                    px: 2
                }}
                >
                <IconButton
                    id="navbar-button"
                    onClick={onSidebarOpen}
                    sx={{
                    display: {
                        xs: 'inline-flex',
                        lg: 'none'
                    }
                    }}
                >
                    <MenuIcon fontSize="small" />
                </IconButton>
                <Tooltip title="Search">
                    <IconButton sx={{ ml: 1 }}>
                    <SearchIcon fontSize="small" />
                    </IconButton>
                </Tooltip>
                <Box sx={{ flexGrow: 1 }} />
                <Tooltip title="Account">
                    <IconButton sx={{ ml: 1 }}>
                    <AccountCircleIcon fontSize="small" />
                    </IconButton>
                </Tooltip>
                <Tooltip title="Notifications">
                    <IconButton sx={{ ml: 1 }}>
                    <Badge
                        badgeContent={4}
                        color="primary"
                        variant="dot"
                    >
                        <NotificationsIcon fontSize="small" />
                    </Badge>
                    </IconButton>
                </Tooltip>
                {/* <Avatar
                    sx={{
                    height: 40,
                    width: 40,
                    ml: 1
                    }}
                    src="/static/images/avatars/avatar_1.png"
                >
                    <AccountCircleIcon fontSize="small" />
                </Avatar> */}
                </Toolbar>
            </NavbarRoot>
        </>
    )
}

Navbar.propTypes = {
    onSidebarOpen: PropTypes.func
};

export default Navbar;
