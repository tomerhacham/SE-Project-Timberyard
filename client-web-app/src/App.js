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
import QueryPage from './components/queries/QueryPage';
// Constants
import {
    CARD_YIELD_PATH, STATION_YIELD_PATH,
    STATION_CARD_YIELD_PATH, SETTINGS_PATH, LOGIN_PATH
} from './constants/constants';

const App = () => {
  return (
    <ThemeProvider theme={theme}>
      <AuthProvider>
        <Router>
          <Routes>
            {/* Private Routes */}
            <Route exact path='/' element={<PrivateRoute />}>
              <Route path='' element={<DashboardLayout />}>
                <Route exact path={SETTINGS_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<Settings />} />
                </Route>
                <Route exact path={CARD_YIELD_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<QueryPage />} />
                </Route>
                <Route exact path={STATION_YIELD_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<QueryPage />} />
                </Route>
                <Route exact path={STATION_CARD_YIELD_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<QueryPage />} />
                </Route>
                <Route exact path='/' element={<Dashboard />} />
              </Route>
            </Route>

            {/* Public Routes */}
            <Route exact path={LOGIN_PATH} element={<Login />} />
          </Routes>
        </Router>
      </AuthProvider>
    </ThemeProvider>
  )
}

export default App;

