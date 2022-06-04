import { useMemo } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { UNAUTHORIZED_CODE } from '../constants/constants';
import axios from 'axios';

const ErrorHandler = ({ children }) => {
    const { logoutAction } = useAuth();

    useMemo(() => {
        axios.interceptors.response.use(
            (response) => response,
            async (error) => {
                if (
                    error.response &&
                    error.response.status === UNAUTHORIZED_CODE
                ) {
                    logoutAction();
                }
                return Promise.reject(error);
            }
        );
    }, [logoutAction]);

    return children;
};

export default ErrorHandler;
