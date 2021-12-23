import React, { useState } from 'react';
import { Container, CssBaseline, Avatar, Typography, TextField, Button,
         Grid, Box, Paper } from '@mui/material';
import SdCardIcon from '@mui/icons-material/SdCard';

const CardYield = () => {
    const [data, setData] = useState({
        catalogNumber: '',
        startDate: '',
        endDate: ''
    })

    const handleSubmit = (e) => {
        e.preventDefault();

        console.log(data);
    };

    return (
        
        // <Grid container component="main" sx={{ height: '100vh' }}>
        <Container component="main" maxWidth={'md'} sx={{ marginLeft: 0 }}>
            <CssBaseline />
            <Grid item xs={12} sm={8} md={5} component={Paper} elevation={6}>
                <Box
                    sx={{
                        my: 4,
                        mx: 2,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                    }}
                >
                    <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
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
                            onChange={(e) => setData({ ...data, catalogNumber: e.target.value })}
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
                            disabled={data.catalogNumber === '' || data.startDate === '' || data.endDate === ''}
                            sx={{ mt: 3, mb: 2 }}
                        >
                            OK
                        </Button>
                    </Box>
                </Box>
            </Grid>
        </Container>
        

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
        //                 onChange={(e) => setData({ ...data, catalogNumber: e.target.value })}
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
