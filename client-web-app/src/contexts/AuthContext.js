import React, { createContext, useContext, useState, useEffect } from 'react'
import PropTypes from 'prop-types';

const AuthContext = createContext();

export const useAuth = () => {
    return useContext(AuthContext);
}

const AuthProvider = ({ children }) => {
    // TODO: think of needed states here
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [accessToken, setAccessToken] = useState({
        token_id: "",
        user_id: "",
        email: ""
    });

    useEffect(() => {
        const tokenFromStorage = localStorage.getItem('token');
        if (tokenFromStorage) {
            setAccessToken(JSON.parse(tokenFromStorage));
            setIsLoggedIn(true);
        }
    }, []);

    const login = (email, password, setErrorMessage) => {
        const mockResponse = { data: {
            token_id: '123ab',
            user_id: '666',
            email
        }};
        setAccessToken(mockResponse.data);
        localStorage.setItem('token', JSON.stringify(mockResponse.data));
        setIsLoggedIn(true);
    };

    const signOut = () => {
        console.log('Signing out');
        localStorage.removeItem('token');
        setIsLoggedIn(false);
    }

    const value = {
        isLoggedIn,
        accessToken,
        login,
        signOut
    };
    
    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}

AuthProvider.propTypes = {
    children: PropTypes.element
}

export default AuthProvider;
