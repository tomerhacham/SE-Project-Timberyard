import React, { useEffect } from 'react'
import PropTypes from 'prop-types';
import { camelCase } from 'lodash';
import { Link, useLocation } from 'react-router-dom';
import { Box, Divider, Drawer, useMediaQuery, List } from '@mui/material';
import { useAuth } from '../../contexts/AuthContext';
import NavItem from '../../generic-components/NavItem';
import Logo from '../icons/Logo';
import DashboardIcon from '@mui/icons-material/Dashboard';
import SettingsIcon from '@mui/icons-material/Settings';
import EvStationIcon from '@mui/icons-material/EvStation';
import LocalGasStationIcon from '@mui/icons-material/LocalGasStation';
import SdCardIcon from '@mui/icons-material/SdCard';
import GppBadIcon from '@mui/icons-material/GppBad';
import FenceIcon from '@mui/icons-material/Fence';
import TimelapseIcon from '@mui/icons-material/Timelapse';
import AvTimerIcon from '@mui/icons-material/AvTimer';
import PowerSettingsNewIcon from '@mui/icons-material/PowerSettingsNew';
import { 
  CARD_YIELD_PATH, STATION_YIELD_PATH, NFF_PATH,
  STATION_CARD_YIELD_PATH, BOUNDARIES_PATH, 
  TESTER_LOAD_PATH, CARD_TEST_DURATION_PATH
} from '../../constants/constants';

const Sidebar = ({ open, onClose }) => {
    const location = useLocation();
    const { signOut } = useAuth();

    const lgUp = useMediaQuery((theme) => theme.breakpoints.up('lg'), {
        defaultMatches: true,
        noSsr: false
    });

    const queriesListItems = [
        {
          primary: 'Station Yield',
          to: STATION_YIELD_PATH,
          icon: <LocalGasStationIcon />
        },
        {
          primary: 'Card Yield',
          to: CARD_YIELD_PATH,
          icon: <SdCardIcon />
        },
        {
          primary: 'Station & Card Yield',
          to: STATION_CARD_YIELD_PATH,
          icon: <EvStationIcon />
        },
        {
          primary: 'NFF',
          to: NFF_PATH,
          icon: <GppBadIcon />
        },
        {
          primary: 'Boundaries',
          to: BOUNDARIES_PATH,
          icon: <FenceIcon />
        },
        {
          primary: 'Tester Load',
          to: TESTER_LOAD_PATH,
          icon: <TimelapseIcon />
        },
        {
          primary: 'Card Test Duration',
          to: CARD_TEST_DURATION_PATH,
          icon: <AvTimerIcon />
        }
    ];
      
    const secondaryListItems = [
        {
          primary: 'Settings',
          to: '/settings',
          icon: <SettingsIcon />
        },
        {
          primary: 'Sign Out',
          to: '/login',
          icon: <PowerSettingsNewIcon />,
          onClick: () => signOut()
        }
    ];

    useEffect(() => {
        if (open) {
            onClose?.();
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [location])

    const content = (
        <>
            <Box
            sx={{
            display: 'flex',
            flexDirection: 'column',
            height: '100%'
            }}
            >
                <div>
                    <Box component={Link} to='/' sx={{ p: 1 }}>
                        <Logo />
                    </Box>
                </div>
                <Divider sx={{ borderColor: '#2D3748', marginBottom: '24px' }} />
                <Box sx={{ flexGrow: 1 }}>
                    <List>
                    <>
                        <NavItem 
                        primary='Dashboard'
                        to='/'
                        icon={<DashboardIcon fontSize='small' />}
                        />
                        {queriesListItems.map((item) =>
                            <NavItem
                                id={`sidebar-${camelCase(item.primary)}`} 
                                key={item.primary}
                                primary={item.primary}
                                to={item.to}
                                icon={item.icon}
                            /> 
                        )}
                    </>
                    </List>
                </Box>
                <Divider sx={{ borderColor: '#2D3748' }} />
                <Box sx={{ flexGrow: 1 }}>
                    <List>
                        {secondaryListItems.map((item) =>
                            <NavItem 
                            key={item.primary}
                            primary={item.primary}
                            to={item.to}
                            icon={item.icon}
                            onClick={'onClick' in item ? item.onClick : null}
                            /> 
                        )}
                    </List>
                </Box>
            </Box>
        </> 
    );

    if (lgUp) {
        return (
          <Drawer
            anchor="left"
            open
            PaperProps={{
              sx: {
                backgroundColor: 'neutral.900',
                color: '#FFFFFF',
                width: 280
              }
            }}
            variant="permanent"
          >
            {content}
          </Drawer>
        );
    }

    return (
        <Drawer
        anchor="left"
        onClose={onClose}
        open={open}
        PaperProps={{
            sx: {
            backgroundColor: 'neutral.900',
            color: '#FFFFFF',
            width: 280
            }
        }}
        sx={{ zIndex: (theme) => theme.zIndex.appBar + 100 }}
        variant="temporary"
        >
            {content}
        </Drawer>
    )
}

Sidebar.propTypes = {
    onClose: PropTypes.func.isRequired,
    open: PropTypes.bool.isRequired
};

export default Sidebar;
