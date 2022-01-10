import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Avatar, Typography, TextField, Button, Grid, Box } from '@mui/material';
import QueryTable from '../dashboard/QueryTable';
import SdCardIcon from '@mui/icons-material/SdCard';
import * as api from '../../api/Api';
import BarChart from './graph/BarChart';

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
    const navigate = useNavigate();
    const [userInput, setUserInput] = useState({
        catalog: '',
        startDate: '',
        endDate: ''
    })
    const [loading, setLoading] = useState(false);
    const [showQuery, setShowQuery] = useState(false);
    const [queryData, setQueryData] = useState(null);
    const [tableData, setTableData] = useState({ rows: [], columns: [] });

    const handleSubmit = (e) => {
        e.preventDefault();
        // console.log(userInput);

        setLoading(true);

        api.CardYield(userInput).then(res => res.json())
        .then(
            (result) => {
                console.log(result);
                setQueryData(result);
                dataToTable(result);
                setLoading(false);
            },
            (error) => {
                // TODO: do some functionality
                console.log(error);
                setLoading(false);
            }
        );
    };

    const dataToTable = (data) => {
        let columns = [];
        data.columnNames.map((headerName) => columns.push({ field: headerName, headerName, width: 150 }));
        // console.log(columns)

        let rows = [];
        data.records.map((record, index) => rows.push({ id: `${index + 1}`, ...record }))
        // console.log(rows);

        setTableData({ rows, columns });
    }

    useEffect(() => {
        setShowQuery(true);
    }, [tableData])

    useEffect(() => {
        setShowQuery(false);
    }, [navigate.pathname])

    const inputFields = (
        <Box
            sx={{
                my: 0,
                mx: 2,
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Avatar sx={{ m: 0, bgcolor: 'secondary.main' }}>
                <SdCardIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
                Card Yield
            </Typography>
            <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 1 }}>
                <TextField
                    id="cardyield-catalog-num"
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
                    id="cardyield-start-date"
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
                    id="cardyield-end-date"
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
                    id="cardyield-submit-button"
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

    return (
        <Box
            component="main"
            sx={{
                flexGrow: 1,
                py: 8
            }}
        >
            <Container maxWidth={false}>
                <Grid container spacing={3}>
                    <Grid item lg={4} md={6} xl={3} xs={12}>
                        {inputFields}
                    </Grid>
                    {loading && (
                        <Typography>Fetching Data...</Typography>
                    )}
                    {showQuery && (
                        <>
                            <Grid item lg={8} md={12} xl={9} xs={12}>
                                <QueryTable rows={tableData.rows} columns={tableData.columns} />
                            </Grid>
                            <Grid item lg={8} md={12} xl={9} xs={12}>
                                {queryData && tableData.rows.length > 0 && <BarChart data={queryData} />}
                            </Grid>
                        </>
                    )}
                </Grid>
            </Container>
        </Box>
    )
}

export default CardYield;
