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
import { AlarmsPost } from '../../../api/Api';

const Alarms = () => {
    const [data, setData] = useState({ data: '' });

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
                    <Grid container spacing={6} wrap='wrap'>
                        <Grid
                            item
                            md={4}
                            sm={6}
                            sx={{
                                display: 'flex',
                                flexDirection: 'column',
                            }}
                            xs={12}>
                            <Typography
                                color='textPrimary'
                                gutterBottom
                                variant='h6'>
                                Alarms
                            </Typography>
                            <FormControl fullWidth>
                                <InputLabel id='select-label'>Type</InputLabel>
                                <Select
                                    labelId='notification-type-select-label'
                                    id='notification-type-select'
                                    value={data.type}
                                    label='Type'
                                    onChange={handleTypeChange}>
                                    <MenuItem value={'Catalog'}>
                                        Catalog
                                    </MenuItem>
                                    <MenuItem value={'Station'}>
                                        Station
                                    </MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>
                        <Grid
                            item
                            md={4}
                            sm={6}
                            sx={{
                                display: 'flex',
                                flexDirection: 'column',
                            }}
                            xs={12}>
                            <Typography
                                color='textPrimary'
                                gutterBottom
                                variant='h6'>
                                Messages
                            </Typography>
                            <FormControlLabel
                                control={
                                    <Checkbox color='primary' defaultChecked />
                                }
                                label='Email'
                            />
                            <FormControlLabel
                                control={<Checkbox />}
                                label='Push Alarms'
                            />
                            <FormControlLabel
                                control={
                                    <Checkbox color='primary' defaultChecked />
                                }
                                label='Phone calls'
                            />
                        </Grid>
                    </Grid>
                </CardContent>
                <Divider />
                <Box
                    sx={{
                        display: 'flex',
                        justifyContent: 'flex-end',
                        p: 2,
                    }}>
                    <Button color='primary' variant='contained'>
                        Save
                    </Button>
                </Box>
            </Card>
        </form>
    );
};

export default Alarms;
