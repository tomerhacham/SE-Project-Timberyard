import React, { useState } from 'react';
import { Card, CardContent, CardHeader, Divider, Grid } from '@mui/material';
import Message from '../../../generic-components/Message';
import AlarmsTable from './AlarmsTable';

const Alarms = () => {
    const [message, setMessage] = useState(null);

    return (
        <form>
            <Card>
                <CardHeader subheader='Manage Alarms' title='Alarms' />
                <Divider />
                {message && (
                    <Message
                        id='alarms-settings-message'
                        text={message.text}
                        severity={message.severity}
                    />
                )}
                <CardContent>
                    <Grid item md={12} sm={12} xs={12}>
                        <AlarmsTable setMessage={setMessage} />
                    </Grid>
                </CardContent>
            </Card>
        </form>
    );
};

export default Alarms;
