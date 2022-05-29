import React, { useState } from 'react';
import {
    Box,
    Button,
    Card,
    CardContent,
    CardHeader,
    Checkbox,
    Divider,
    FormControlLabel,
    FormControl,
    Grid,
    Typography,
    Select,
    MenuItem,
    InputLabel,
} from '@mui/material';
import AlarmsTable from './AlarmsTable';

const Alarms = () => {
    const [data, setData] = useState({ type: 0 });

    const handleTypeChange = (event) => {
        setData({ type: event.target.value, ...data });
        // console.log(event.target.value);
    };

    return (
        <form>
            <Card>
                <CardHeader subheader='Manage Alarms' title='Alarms' />
                <Divider />
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
                        <AlarmsTable />
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
