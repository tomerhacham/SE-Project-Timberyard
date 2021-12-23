import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Container, CssBaseline, Avatar, Typography, TextField, Button,
    Grid, Box, Paper
} from '@mui/material';
import SdCardIcon from '@mui/icons-material/SdCard';
import * as api from '../../api/Api';
import QueryTable from '../dashboard/QueryTable';

// const rowsExample = [
//     { id: 1, col1: 'Hello', col2: 'World' },
//     { id: 2, col1: 'DataGridPro', col2: 'is Awesome' },
//     { id: 3, col1: 'MUI', col2: 'is Amazing' },
// ];

// const columnsExample = [
//     { field: 'col1', headerName: 'Column 1', width: 150 },
//     { field: 'col2', headerName: 'Column 2', width: 150 },
// ];

const mockData = [
    {
        "columnNames": [
            "catalog",
            "cardName",
            "successRatio"
        ],
        "records": [
            {
                "catalog": "X56868",
                "cardName": "OA_HF",
                "successRatio": 93.12
            },
            {
                "catalog": "X56868",
                "cardName": "OP_KLF",
                "successRatio": 95.2
            },
            {
                "catalog": "X56868",
                "cardName": "OA_ASDF",
                "successRatio": 89.2
            },
            {
                "catalog": "X56868",
                "cardName": "OA_HF2",
                "successRatio": 100
            },
            {
                "catalog": "X56868",
                "cardName": "OP_KLF2",
                "successRatio": 72
            },
            {
                "catalog": "X56868",
                "cardName": "OA_ASDF2",
                "successRatio": 76
            },
            {
                "catalog": "X56868",
                "cardName": "OA_HF3",
                "successRatio": 77
            },
            {
                "catalog": "X56868",
                "cardName": "OP_KLF3",
                "successRatio": 79
            },
            {
                "catalog": "X56868",
                "cardName": "OA_ASDF3",
                "successRatio": 50
            }
        ]
    }
];

const CardYield = () => {
    const navigate = useNavigate();
    const [data, setData] = useState({
        catalog: '',
        startDate: '',
        endDate: ''
    })
    const [showQuery, setShowQuery] = useState(false);
    const [tableData, setTableData] = useState({
        rows: [],
        columns: []
    })

    const handleSubmit = (e) => {
        e.preventDefault();
        console.log(data);

        // api.CardYield(data).then(res => res.json())
        // .then(
        //   (result) => {
        //     console.log(result)
        //   },
        //   (error) => {
        //     console.log(error)
        //   }
        // );

        dataToTable(mockData);
    };

    const dataToTable = (data) => {
        let columns = [];
        data[0].columnNames.map((headerName) => columns.push({ field: headerName, headerName, width: 150 }));
        // console.log(columns)

        let rows = [];
        data[0].records.map((record, index) => rows.push({ id: `${index + 1}`, ...record }))
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
                            onChange={(e) => setData({ ...data, catalog: e.target.value })}
                        />
                        <TextField
                            id="cardyield-start-date"
                            required
                            variant="outlined"
                            margin="normal"
                            fullWidth
                            label="Start Date"
                            type="date"
                            onChange={(e) => setData({ ...data, startDate: e.target.value })}
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
                            onChange={(e) => setData({ ...data, endDate: e.target.value })}
                            InputLabelProps={{ shrink: true }}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            disabled={data.catalog === '' || data.startDate === '' || data.endDate === ''}
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
                <Grid
                    container
                    spacing={3}
                >
                    <Grid
                        item
                        lg={4}
                        md={6}
                        xl={3}
                        xs={12}
                    >
                        {inputFields}
                    </Grid>
                    {showQuery && (
                        <Grid
                            item
                            lg={8}
                            md={12}
                            xl={9}
                            xs={12}
                        >
                            <QueryTable rows={tableData.rows} columns={tableData.columns} />
                        </Grid>
                    )}
                </Grid>
            </Container>
        </Box>



        // <Container component="main" maxWidth={'md'} sx={{ marginLeft: 0 }}>
        //     <CssBaseline />
        //     <Grid item xs={12} sm={8} md={5} component={Paper} elevation={6}>
        //         <Box
        //             sx={{
        //                 my: 4,
        //                 mx: 2,
        //                 display: 'flex',
        //                 flexDirection: 'column',
        //                 alignItems: 'center',
        //             }}
        //         >
        //             <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
        //                 <SdCardIcon />
        //             </Avatar>
        //             <Typography component="h1" variant="h5">
        //                 Card Yield
        //             </Typography>
        //             <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 1 }}>
        //                 <TextField
        //                     id="cardyield-catalog-num"
        //                     required
        //                     variant="outlined"
        //                     margin="normal"
        //                     fullWidth
        //                     label="Catalog #"
        //                     type="text"
        //                     autoFocus
        //                     onChange={(e) => setData({ ...data, catalog: e.target.value })}
        //                 />
        //                 <TextField
        //                     id="cardyield-start-date"
        //                     required
        //                     variant="outlined"
        //                     margin="normal"
        //                     fullWidth
        //                     label="Start Date"
        //                     type="date"
        //                     onChange={(e) => setData({ ...data, startDate: e.target.value })}
        //                     InputLabelProps={{ shrink: true }}
        //                 />
        //                 <TextField
        //                     id="cardyield-end-date"
        //                     required
        //                     variant="outlined"
        //                     margin="normal"
        //                     fullWidth
        //                     label="End Date"
        //                     type="date"
        //                     onChange={(e) => setData({ ...data, endDate: e.target.value })}
        //                     InputLabelProps={{ shrink: true }}
        //                 />
        //                 <Button
        //                     type="submit"
        //                     fullWidth
        //                     variant="contained"
        //                     disabled={data.catalog === '' || data.startDate === '' || data.endDate === ''}
        //                     sx={{ mt: 3, mb: 2 }}
        //                 >
        //                     OK
        //                 </Button>
        //             </Box>
        //         </Box>
        //         {showQuery && (
        //             <QueryTable rows={tableData.rows} columns={tableData.columns} />
        //         )}
        //     </Grid>
        // </Container>


        // <Container component="main" maxWidth="xs">
        //     <CssBaseline />
        //     <div>
        //         {/* <Avatar>
        //             H
        //         </Avatar> */}
        //         <Typography component="h1" variant="h5">
        //             Card Yield
        //         </Typography>
        //         <form onSubmit={handleSubmit}>
        //             <TextField
        //                 id="cardyield-catalog-num"
        //                 required
        //                 variant="outlined"
        //                 margin="normal"
        //                 fullWidth
        //                 label="Catalog #"
        //                 type="text"
        //                 autoFocus
        //                 onChange={(e) => setData({ ...data, catalog: e.target.value })}
        //             />
        //             <TextField
        //                 id="cardyield-start-date"
        //                 required
        //                 variant="outlined"
        //                 margin="normal"
        //                 fullWidth
        //                 label="Start Date"
        //                 type="date"
        //                 onChange={(e) => setData({ ...data, startDate: e.target.value })}
        //                 InputLabelProps={{ shrink: true }}
        //             />
        //             <TextField
        //                 id="cardyield-end-date"
        //                 required
        //                 variant="outlined"
        //                 margin="normal"
        //                 fullWidth
        //                 label="End Date"
        //                 type="date"
        //                 onChange={(e) => setData({ ...data, endDate: e.target.value })}
        //                 InputLabelProps={{ shrink: true }}
        //             />
        //             {/* OK Button */}
        //             <Button
        //                 type="submit"
        //                 fullWidth
        //                 variant="contained"
        //                 color="primary"
        //             >
        //                 OK
        //             </Button>
        //         </form>
        //     </div>
        // </Container>
    )
}

export default CardYield;
