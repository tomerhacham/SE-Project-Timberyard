import React, { useState, useEffect, Fragment } from 'react';
import { Container, Avatar, Typography, TextField, Button, Grid, Box } from '@mui/material';
import SdCardIcon from '@mui/icons-material/SdCard';
import QueryTable from '../dashboard/QueryTable';
import { QueryPost } from '../../api/Api';
import Loader from '../../generic-components/Loader';
import BarChart from './graph/BarChart';
import { dataToTable } from '../../utils/helperFunctions';
import { CARD_YIELD_URL } from '../../constants/api-urls';
import { CARD_YIELD_TITLE } from '../../constants/queries';
import { queriesInputBoxSx } from '../../theme';

// const rowsExample = [
//     { id: 1, col1: 'Hello', col2: 'World' },
//     { id: 2, col1: 'DataGridPro', col2: 'is Awesome' },
//     { id: 3, col1: 'MUI', col2: 'is Amazing' },
// ];

// const columnsExample = [
//     { field: 'col1', headerName: 'Column 1', width: 150 },
//     { field: 'col2', headerName: 'Column 2', width: 150 },
// ];

const CardYield = () => {
    const [userInput, setUserInput] = useState({ catalog: '', startDate: '', endDate: '' })
    const [loading, setLoading] = useState(false);
    const [showQuery, setShowQuery] = useState(false);
    const [queryData, setQueryData] = useState(null);
    const [tableData, setTableData] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();

        setLoading(true);

        const request = { url: CARD_YIELD_URL, data: userInput };
        const result = await QueryPost(request);
        if (result) {
            console.log(result);
            setQueryData(result);
            setTableData(dataToTable(result));
        }
        setLoading(false);
        
        // QueryPost(request).then(res => res.json())
        // .then((result) => {
        //     console.log(result);
        //     setQueryData(result);
        //     setTableData(dataToTable(result));
        // })
        // .catch((error) => {
        //     console.log('Catched error:', error);
        // })
        // .finally(() => setLoading(false));
    };

    const inputFields = (
        <Box id='input-box' sx={queriesInputBoxSx}>
            <Avatar sx={{ m: 0, bgcolor: 'secondary.main' }}>
                <SdCardIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
                {CARD_YIELD_TITLE}
            </Typography>
            <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 1 }}>
                <TextField
                    id="card-yield-catalog"
                    required
                    variant="outlined"
                    margin="normal"
                    fullWidth
                    label="Catalog #"
                    type="text"
                    autoFocus
                    onChange={(e) => setUserInput({ ...userInput, catalog: e.target.value })}
                />
                <TextField
                    id="card-yield-start-date"
                    required
                    variant="outlined"
                    margin="normal"
                    fullWidth
                    label="Start Date"
                    type="date"
                    onChange={(e) => setUserInput({ ...userInput, startDate: e.target.value })}
                    InputLabelProps={{ shrink: true }}
                />
                <TextField
                    id="card-yield-end-date"
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
                    id="card-yield-submit-button"
                    type="submit"
                    fullWidth
                    variant="contained"
                    disabled={userInput.catalog === '' || userInput.startDate === '' || userInput.endDate === ''}
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
            id="card-yield-box"
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
                        <Fragment>
                            <Grid item lg={8} md={12} xl={9} xs={12}>
                                <QueryTable rows={tableData.rows} columns={tableData.columns} />
                            </Grid>
                            <Grid item lg={8} md={12} xl={9} xs={12}>
                                {queryData && tableData.rows.length > 0 && 
                                    <BarChart data={queryData} />}
                            </Grid>
                        </Fragment>
                    )}
                </Grid>
            </Container>
        </Box>
    )
}

export default CardYield;
