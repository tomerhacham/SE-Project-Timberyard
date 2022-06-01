import React, {
    createContext,
    useContext,
    useState,
    useEffect,
    useMemo,
    useCallback,
} from 'react';
import PropTypes from 'prop-types';
import { ROLE, SUCCESS_CODE } from '../constants/constants';
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

    const handleLogin = useCallback(async (email, password, role) => {
        const response = await Login({ email, password });
        if (
            response &&
            response.status === SUCCESS_CODE &&
            response.data !== ''
        ) {
            const token = {
                ...accessToken,
                role,
                token: response.data.token,
            };
            setAccessToken(token);
            localStorage.setItem('access_token', JSON.stringify(token));
            setIsLoggedIn(true);
        } else {
            // TODO: Handle error
            console.log('Login failed.');
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
            loginAction: (email, password, role) =>
                handleLogin(email, password, role),
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
