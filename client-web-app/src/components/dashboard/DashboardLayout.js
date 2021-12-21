import React, { useState } from 'react';
import { styled } from '@mui/material/styles';
import { Box } from '@mui/material';
import Navbar from './Navbar';
import Sidebar from './Sidebar';
import { Outlet } from 'react-router-dom';

const DashboardLayoutRoot = styled('div')(({ theme }) => ({
  display: 'flex',
  flex: '1 1 auto',
  maxWidth: '100%',
  paddingTop: 64,
  [theme.breakpoints.up('lg')]: {
    paddingLeft: 280
  }
}));

const DashboardLayout = () => {
  const [isSidebarOpen, setSidebarOpen] = useState(true);

  return (
    <>
      <DashboardLayoutRoot>
        <Box
          sx={{
            display: 'flex',
            flex: '1 1 auto',
            flexDirection: 'column',
            width: '100%'
          }}
        >
          <Outlet />
        </Box>
      </DashboardLayoutRoot>
      <Navbar onSidebarOpen={() => setSidebarOpen(true)} />
      <Sidebar open={isSidebarOpen} onClose={() => setSidebarOpen(false)} />
    </>
  )
}

export default DashboardLayout;
