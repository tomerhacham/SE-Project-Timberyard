import React, { useState, Fragment } from 'react';
import { useMemo } from 'react';
import axios from 'axios';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
} from '@mui/material';
import { useAuth } from '../contexts/AuthContext';
import Message from '../generic-components/Message';
import {
    MESSAGE,
    UNAUTHORIZED_CODE,
    BAD_REQUEST_CODE,
    ERR_NETWORK_CODE,
} from '../constants/constants';

const DialogScreen = (props) => {
    const { onClose, open, message } = props;

    const handleClose = () => {
        onClose();
    };

    return (
        <Dialog
            id='error-handler-dialog'
            fullWidth
            onClose={handleClose}
            open={open}>
            <DialogTitle>ERROR</DialogTitle>
            <DialogContent>
                <Message
                    id='error-handler-message'
                    style={{ marginTop: '10px' }}
                    text={message.text}
                    severity={message.severity}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose}>Close</Button>
            </DialogActions>
        </Dialog>
    );
};

const ErrorHandler = ({ children }) => {
    const { logoutAction, apiMessage, setApiMessage } = useAuth();
    const [open, setOpen] = useState(false);

    const handleClose = () => {
        setOpen(false);
        setApiMessage(null);
    };

    useMemo(() => {
        axios.interceptors.response.use(
            (response) => response,
            async (error) => {
                if (error.response) {
                    const status = error.response.status;
                    if (error.code === ERR_NETWORK_CODE) {
                        logoutAction();
                        setApiMessage({
                            text: 'Server Network Error.\nCheck console for more details.',
                            severity: MESSAGE.ERROR,
                        });
                        setOpen(true);
                    }
                    if (status === UNAUTHORIZED_CODE) {
                        // 401
                        logoutAction();
                        setApiMessage({
                            text: 'Unauthorized. Please login.',
                            severity: MESSAGE.WARNING,
                        });
                        setOpen(true);
                    } else if (status >= BAD_REQUEST_CODE) {
                        // 400, 402...
                        setApiMessage({
                            text: error.response.message || error.message,
                            severity: MESSAGE.ERROR,
                        });
                        setOpen(true);
                    }
                }
                return Promise.reject(error);
            }
        );
    }, [logoutAction, setApiMessage]);

    return (
        <Fragment>
            {children}
            {apiMessage && (
                <DialogScreen
                    open={open}
                    onClose={handleClose}
                    message={apiMessage}
                />
            )}
        </Fragment>
    );
};

export default ErrorHandler;
