import React, { useState, useEffect, Fragment } from 'react';
import { useLocation } from 'react-router-dom';
import { some, keys } from 'lodash';
import { Container, Avatar, Typography, TextField, Button, Grid, Box } from '@mui/material';
import SdCardIcon from '@mui/icons-material/SdCard';
import LocalGasStationIcon from '@mui/icons-material/LocalGasStation';
import EvStationIcon from '@mui/icons-material/EvStation';
import { QueryPost } from '../../api/Api';
import QueryTable from './QueryTable';
import BarChart from './graph/BarChart';
import Loader from '../../generic-components/Loader';
import { dataToTable } from '../../utils/helperFunctions';
import queriesJson from '../../json/queriesPages.json';
import { queriesInputBoxSx } from '../../theme';
import { 
    CARD_YIELD_PATH, STATION_YIELD_PATH, 
    STATION_CARD_YIELD_PATH, CARD_YIELD_ID
} from '../../constants/constants';

const QueryPage = () => {
    const location = useLocation();

    const [queryElement, setQueryElement] = useState(null);
    const { id, title, fields, url, icon } = queryElement ?? {};

    const [userInput, setUserInput] = useState({});
    const [loading, setLoading] = useState(false);
    const [showQuery, setShowQuery] = useState(false);
    const [tableData, setTableData] = useState(null);
    const [queryData, setQueryData] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        setLoading(true);
        const request = { url, data: userInput };
        const result = await QueryPost(request);
        if (result) {
            console.log(result);
            setTableData(dataToTable(result));
            id === CARD_YIELD_ID && setQueryData(result);
        }
    };

    const isButtonDisabled = () => {
        if (!fields || keys(userInput).length < fields.length) {
            return true;
        }
        return some(userInput, (field) => field === '');
    }

    const renderIcon = () => {
        switch (icon) {
            case 'SdCard':
                return <SdCardIcon />;
            case 'LocalGasStation':
                return <LocalGasStationIcon />;
            case 'EvStation':
                return <EvStationIcon />;
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
                        onChange={(e) => setUserInput({ ...userInput, [field.id]: e.target.value })}
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
        setShowQuery(false);
        setTableData(null);
        setQueryData(null);
        setUserInput({});

        switch (location.pathname) {
            case CARD_YIELD_PATH:
                setQueryElement(queriesJson.cardYield);
                break;
            case STATION_YIELD_PATH:
                setQueryElement(queriesJson.stationYield);
                break;
            case STATION_CARD_YIELD_PATH:
                setQueryElement(queriesJson.stationCardYield);
                break;
            default:
                console.log('error in location');
        }
    }, [location.pathname]);

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
                            {id === CARD_YIELD_ID &&
                                <Grid item lg={8} md={12} xl={9} xs={12}>
                                    {queryData && tableData.rows.length > 0 && 
                                        <BarChart data={queryData} />}
                                </Grid>}
                        </Fragment>
                    )}
                </Grid>
            </Container>
        </Box>
    )
}

export default QueryPage;