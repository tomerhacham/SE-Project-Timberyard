import React, {
    createContext,
    useContext,
    useState,
    useEffect,
    useMemo,
    useCallback,
} from 'react';
import PropTypes from 'prop-types';
import { ROLE } from '../constants/constants';
import { Login } from '../api/Api';

const initialTokenState = {
    // loginRequested: false,
    // logoutRequested: false,
    // isLoggedIn: false,
    // credentials: {},
    // errorMessage: '',
    // apiResponse: {},
    // openApiErrorDialog: false,
    role: ROLE.UNAUTHORIZE,
    token: '',
};

const AuthContext = createContext();

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context !== undefined) {
        return context;
    }
};

const AuthProvider = ({ children }) => {
    const [accessToken, setAccessToken] = useState(initialTokenState);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [apiResponse, setApiResponse] = useState({});

    useEffect(() => {
        const tokenFromStorage = localStorage.getItem('access_token');
        if (tokenFromStorage) {
            setAccessToken(JSON.parse(tokenFromStorage));
            setIsLoggedIn(true);
        }
    }, []);

    const handleLogin = useCallback(async (email, password, setMessage) => {
        const result = await Login({ email, password });
        if (result && result.status && result.data?.token) {
            const token = {
                ...accessToken,
                role: result.data.role,
                token: result.data.token,
            };
            setAccessToken(token);
            localStorage.setItem('access_token', JSON.stringify(token));
            setIsLoggedIn(true);
        } else if (result && !result.status) {
            setMessage({
                text: result.message,
                severity: 'error',
            });
        } else {
            console.log('Error in login');
        }
    }, []);

    const handleLogout = useCallback(() => {
        localStorage.removeItem('access_token');
        setIsLoggedIn(false);
        setAccessToken(initialTokenState);
    }, []);

    const authContextValue = useMemo(() => {
        return {
            accessToken,
            isLoggedIn,
            apiResponse,
            setApiResponse,
            loginAction: (email, password, setMessage) =>
                handleLogin(email, password, setMessage),
            logoutAction: () => handleLogout(),
        };
    }, [
        accessToken,
        isLoggedIn,
        apiResponse,
        setApiResponse,
        handleLogin,
        handleLogout,
    ]);

    return (
        <AuthContext.Provider value={authContextValue}>
            {children}
        </AuthContext.Provider>
    );
};

AuthProvider.propTypes = {
    children: PropTypes.element,
};

export default AuthProvider;
