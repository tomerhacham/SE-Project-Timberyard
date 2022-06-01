import React, { useState } from 'react';
import { Card, CardContent, CardHeader, Divider, Grid } from '@mui/material';
import Message from '../../../generic-components/Message';
import AlarmsTable from './AlarmsTable';

const Alarms = () => {
    const [data, setData] = useState({ type: 0 });
    const [message, setMessage] = useState(null);

    const handleTypeChange = (event) => {
        setData({ type: event.target.value, ...data });
        // console.log(event.target.value);
    };

    return (
        <form>
            <Card>
                <CardHeader subheader='Manage Alarms' title='Alarms' />
                <Divider />
                {message && (
                    <Message text={message.text} severity={message.severity} />
                )}
                <CardContent>
                    {/* <Grid container wrap='wrap'> */}
                    <Grid
                        item
                        md={12}
                        sm={12}
                        // sx={{
                        //     display: 'flex',
                        //     flexDirection: 'column',
                        // }}
                        xs={12}>
                        <AlarmsTable setMessage={setMessage} />
                    </Grid>
                    {/* </Grid> */}
                </CardContent>
                {/* <Divider />
                <Box
                    sx={{
                        display: 'flex',
                        justifyContent: 'flex-end',
                        p: 2,
                    }}>
                    <Button color='primary' variant='contained'>
                        Save
                    </Button>
                </Box> */}
            </Card>
        </form>
    );
};

export default Alarms;
