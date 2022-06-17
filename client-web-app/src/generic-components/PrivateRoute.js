import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import Loader from './Loader';

const PrivateRoute = () => {
    const { isLoggedIn } = useAuth();

    if (isLoggedIn === undefined) {
        return <Loader />;
    }

    return isLoggedIn ? <Outlet /> : <Navigate to='/login' />;
};

export default PrivateRoute;
