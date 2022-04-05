import React from 'react';
import { Box, Container, Grid } from '@mui/material';

// TODO
const DashboardApp = () => {
    return (
        <Box
            component="main"
            sx={{ flexGrow: 1, py: 8 }}
        >
          <Container maxWidth={false}>
            <Grid container spacing={3}>
              <Grid item lg={8} sm={12} xl={9} xs={12}>
                Dashboard
              </Grid>
            </Grid>
          </Container>
        </Box>
    )
}

export default DashboardApp;
