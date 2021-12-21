import React from 'react';
import { Routes, Route, BrowserRouter as Router } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
import { theme } from './theme/index';
import AuthProvider from './contexts/AuthContext';
import PrivateRoute from './utils/PrivateRoute';
// Pages
import DashboardLayout from './components/dashboard/DashboardLayout';
import Dashboard from './components/dashboard/Dashboard';
import Login from './components/login/Login';
import Settings from './components/settings/Settings';

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

