import React from 'react';
import PropTypes from 'prop-types';
import { capitalize } from 'lodash';
import { Alert } from '@mui/material';

const Message = (props) => {
    const { text, severity, style } = props;

    const renderText = () => {
        if (text && text !== '') {
            return text;
        }
        return capitalize(severity);
    };

    return (
        <Alert style={style} severity={severity}>
            {renderText()}
        </Alert>
    );
};

Message.propTypes = {
    text: PropTypes.string,
    severity: PropTypes.oneOf(['error', 'success', 'info', 'warning'])
        .isRequired,
};

export default Message;
