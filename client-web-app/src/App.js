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
import queriesJson from './components/queries/json/queries.json';
// Constants
import {
    CARD_YIELD_PATH, STATION_YIELD_PATH, NFF_PATH,
    STATION_CARD_YIELD_PATH, SETTINGS_PATH, LOGIN_PATH,
    BOUNDARIES_PATH
} from './constants/constants';

const App = () => {
  return (
    <ThemeProvider theme={theme}>
      <AuthProvider>
        <Router>
          <Routes>
            {/* Private Routes */}
            <Route path='' element={<DashboardLayout />}>
              <Route element={<PrivateRoute />}>
                <Route path={SETTINGS_PATH} element={<Settings />} />
                <Route path={CARD_YIELD_PATH} element={<QueryPage data={queriesJson.cardYield} />} />
                <Route path={STATION_YIELD_PATH} element={<QueryPage data={queriesJson.stationYield} />} />
                <Route path={STATION_CARD_YIELD_PATH} element={<QueryPage data={queriesJson.stationCardYield} />} />
                <Route path={NFF_PATH} element={<QueryPage data={queriesJson.nff} />} />
                <Route path={BOUNDARIES_PATH} element={<QueryPage data={queriesJson.boundaries} />} />
                <Route exact path='/' element={<Dashboard />} />
              </Route>
            </Route>

            {/* TODO: Check if can delete */}
            
            {/* <Route exact path='/' element={<PrivateRoute />}>
              <Route path='' element={<DashboardLayout />}>
                <Route exact path={SETTINGS_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<Settings />} />
                </Route>
                <Route exact path={CARD_YIELD_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<QueryPage data={queriesJson.cardYield} />} />
                </Route>
                <Route exact path={STATION_YIELD_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<QueryPage data={queriesJson.stationYield} />} />
                </Route>
                <Route exact path={STATION_CARD_YIELD_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<QueryPage data={queriesJson.stationCardYield} />} />
                </Route>
                <Route exact path={NFF_PATH} element={<PrivateRoute />}>
                  <Route path='' element={<QueryPage data={queriesJson.nff} />} />
                </Route>
                <Route exact path='/' element={<Dashboard />} />
              </Route>
            </Route> */}

            {/* Public Routes */}
            <Route exact path={LOGIN_PATH} element={<Login />} />
          </Routes>
        </Router>
      </AuthProvider>
    </ThemeProvider>
  )
}

export default App;

