import React from 'react';
import { Routes, Route, BrowserRouter as Router } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
import { theme } from './theme/index';
import AuthProvider from './contexts/AuthContext';
import PrivateRoute from './generic-components/PrivateRoute';
// Pages
import DashboardLayout from './components/dashboard/DashboardLayout';
import Dashboard from './components/dashboard/Dashboard';
import Login from './components/login/Login';
import Settings from './components/settings/Settings';
import CardYield from './components/queries/CardYield';
import StationYield from './components/queries/StationYield';
import StationCardYield from './components/queries/StationCardYield';

const App = () => {
  return (
    <ThemeProvider theme={theme}>
      <AuthProvider>
        <Router>
          <Routes>
            {/* Private Routes */}
            <Route exact path='/' element={<PrivateRoute />}>
              <Route path='' element={<DashboardLayout />}>
                <Route exact path='/settings' element={<PrivateRoute />}>
                  <Route path='' element={<Settings />} />
                </Route>
                <Route exact path='/cardyield' element={<PrivateRoute />}>
                  <Route path='' element={<CardYield />} />
                </Route>
                <Route exact path='/stationyield' element={<PrivateRoute />}>
                  <Route path='' element={<StationYield />} />
                </Route>
                <Route exact path='/stationcardyield' element={<PrivateRoute />}>
                  <Route path='' element={<StationCardYield />} />
                </Route>
                <Route exact path='/' element={<Dashboard />} />
              </Route>
            </Route>

            {/* Public Routes */}
            <Route exact path='/login' element={<Login />} />
          </Routes>
        </Router>
      </AuthProvider>
    </ThemeProvider>
  )
}

export default App;

