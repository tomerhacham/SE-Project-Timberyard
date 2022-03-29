import React, { useState, useEffect } from 'react';
import { Container, Avatar, Typography, TextField, Button, Grid, Box } from '@mui/material';
import LocalGasStationIcon from '@mui/icons-material/LocalGasStation';
import QueryTable from '../../generic-components/QueryTable';
import { QueryPost } from '../../api/Api';
import Loader from '../../generic-components/Loader';
import { dataToTable } from '../../utils/helperFunctions';
import { STATION_YIELD_URL } from '../../constants/api-urls';
import { STATION_YIELD_TITLE } from '../../constants/queries';
import { queriesInputBoxSx } from '../../theme';

const StationYield = () => {
    const [userInput, setUserInput] = useState({ startDate: '', endDate: '' });
    const [loading, setLoading] = useState(false);
    const [showQuery, setShowQuery] = useState(false);
    const [tableData, setTableData] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        setLoading(true);

        const request = { url: STATION_YIELD_URL, data: userInput };
        const result = await QueryPost(request);
        if (result) {
            console.log(result);
            setTableData(dataToTable(result));
        }
        setLoading(false);
    };

    const inputFields = (
        <Box id='input-box' sx={queriesInputBoxSx}>
            <Avatar sx={{ m: 0, bgcolor: 'secondary.main' }}>
                <LocalGasStationIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
                {STATION_YIELD_TITLE}
            </Typography>
            <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 1 }}>
                <TextField
                    id="station-yield-start-date"
                    required
                    variant="outlined"
                    margin="normal"
                    fullWidth
                    label="Start Date"
                    type="date"
                    autoFocus
                    onChange={(e) => setUserInput({ ...userInput, startDate: e.target.value })}
                    InputLabelProps={{ shrink: true }}
                />
                <TextField
                    id="station-yield-end-date"
                    required
                    variant="outlined"
                    margin="normal"
                    fullWidth
                    label="End Date"
                    type="date"
                    onChange={(e) => setUserInput({ ...userInput, endDate: e.target.value })}
                    InputLabelProps={{ shrink: true }}
                />
                <Button
                    id="station-yield-submit-button"
                    type="submit"
                    fullWidth
                    variant="contained"
                    disabled={userInput.startDate === '' || userInput.endDate === ''}
                    sx={{ mt: 3, mb: 2 }}
                >
                    OK
                </Button>
            </Box>
        </Box>
    )

    useEffect(() => {
        if (tableData) {
            setShowQuery(true);
        }
    }, [tableData]);

    return (
        <Box 
            id='station-yield-box'
            component="main"
            sx={{ flexGrow: 1, py: 8 }}
        >
            <Container maxWidth={false}>
                <Grid container spacing={3}>
                    <Grid item lg={4} md={6} xl={3} xs={12}>
                        {inputFields}
                    </Grid>
                    {loading && (
                        <Loader />
                    )}
                    {showQuery && (
                        <Grid item lg={8} md={12} xl={9} xs={12}>
                            <QueryTable rows={tableData.rows} columns={tableData.columns} />
                        </Grid>
                    )}
                </Grid>
            </Container>
        </Box>
    )
}

export default StationYield;