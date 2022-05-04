import React, { useState, useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import { some, keys } from 'lodash';
import { Container, Avatar, Typography, TextField, Button, Grid, Box } from '@mui/material';
import SdCardIcon from '@mui/icons-material/SdCard';
import LocalGasStationIcon from '@mui/icons-material/LocalGasStation';
import EvStationIcon from '@mui/icons-material/EvStation';
import GppBadIcon from '@mui/icons-material/GppBad';
import FenceIcon from '@mui/icons-material/Fence';
import { QueryPost } from '../../api/Api';
import QueryTable from './QueryTable';
import BarChart from './graph/BarChart';
import Loader from '../../generic-components/Loader';
import { dataToTable } from '../../utils/helperFunctions';
import { queriesInputBoxSx } from '../../theme';
import {  
    CARD_YIELD_ID, CARD_YIELD_ICON, STATION_YIELD_ICON,
    STATION_CARD_YIELD_ICON, NFF_ICON, BOUNDARIES_ICON
} from '../../constants/constants';

const QueryPage = ({ data }) => {
    const [queryElement, setQueryElement] = useState(data);
    const { id, title, fields, url, icon } = queryElement ?? {};

    const [userInput, setUserInput] = useState({});
    const [loading, setLoading] = useState(false);
    const [showQuery, setShowQuery] = useState(false);
    const [tableData, setTableData] = useState(null);
    const [chartData, setChartData] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        setLoading(true);
        const request = { url, data: userInput };
        const result = await QueryPost(request);
        if (result) {
            console.log(result);
            setTableData(dataToTable(result));
            showBarChart() && setChartData(result);
        }
    };

    const isButtonDisabled = () => {
        if (!fields || keys(userInput).length < fields.length) {
            return true;
        }
        return some(userInput, (field) => field === '');
    }

    // TODO: Check if should be in other queries too
    const showBarChart = () => id === CARD_YIELD_ID;

    const renderIcon = () => {
        switch (icon) {
            case CARD_YIELD_ICON:
                return <SdCardIcon />;
            case STATION_YIELD_ICON:
                return <LocalGasStationIcon />;
            case STATION_CARD_YIELD_ICON:
                return <EvStationIcon />;
            case NFF_ICON:
                return <GppBadIcon />;
            case BOUNDARIES_ICON:
                return <FenceIcon />;
            default:
                return <SdCardIcon />;
        }
    }

    const inputFields = (
        <Box id={`${id}-input-box`} sx={queriesInputBoxSx}>
            {icon && 
                <Avatar sx={{ m: 0, bgcolor: 'secondary.main' }}>
                    {renderIcon()}
                </Avatar>
            }
            <Typography component="h1" variant="h5">
                {title}
            </Typography>
            <Box key={id} component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 1 }}>
                {fields && fields.map((field, index) => 
                    <TextField
                        key={index}
                        id={`${id}-${field.id}`}
                        required={field.required || false }
                        variant="outlined"
                        margin="normal"
                        fullWidth
                        label={field.label}
                        type={field.type}
                        autoFocus={field.autoFocus || false}
                        onChange={(e) => 
                            setUserInput({ ...userInput, 
                                [field.id]: field.type === 'number' ? e.target.valueAsNumber : e.target.value 
                            })}
                        InputLabelProps={{ shrink: true }}
                    />
                )}
                <Button
                    id={`${id}-submit-button`}
                    type="submit"
                    fullWidth
                    variant="contained"
                    disabled={isButtonDisabled()}
                    sx={{ mt: 3, mb: 2 }}
                >
                    OK
                </Button>
            </Box>
        </Box>
    )

    useEffect(() => {
        if (tableData) {
            setLoading(false);
            setShowQuery(true);
        }
    }, [tableData]);

    useEffect(() => {
        setQueryElement(data);
    }, [data]);

    useEffect(() => {
        // reset fields on page change
        setShowQuery(false);
        setLoading(false);
        setTableData(null);
        setChartData(null);
        setUserInput({});
    }, [queryElement]);

    return (
        <Box 
            id='query-page-box'
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
                            {showBarChart() &&
                                <Grid item lg={8} md={12} xl={9} xs={12}>
                                    {chartData && tableData.rows.length > 0 && 
                                        <BarChart data={chartData} />}
                                </Grid>}
                        </Fragment>
                    )}
                </Grid>
            </Container>
        </Box>
    )
}

QueryPage.propTypes = {
    data: PropTypes.shape({
        id: PropTypes.string.isRequired,
        title: PropTypes.string.isRequired,
        url: PropTypes.string.isRequired,
        fields: PropTypes.arrayOf(PropTypes.shape({
            id: PropTypes.string.isRequired,
            label: PropTypes.string.isRequired,
            required: PropTypes.bool,
            type: PropTypes.string.isRequired,
            autoFocus: PropTypes.bool
        }))
    })
};

export default QueryPage;