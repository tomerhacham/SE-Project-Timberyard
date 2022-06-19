import React, { useState, useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import { some, keys, startCase } from 'lodash';
import {
    Container,
    Avatar,
    Typography,
    TextField,
    Button,
    Grid,
    Box,
} from '@mui/material';
import SdCardIcon from '@mui/icons-material/SdCard';
import LocalGasStationIcon from '@mui/icons-material/LocalGasStation';
import EvStationIcon from '@mui/icons-material/EvStation';
import GppBadIcon from '@mui/icons-material/GppBad';
import FenceIcon from '@mui/icons-material/Fence';
import TimelapseIcon from '@mui/icons-material/Timelapse';
import AvTimerIcon from '@mui/icons-material/AvTimer';
import { QueryPost } from '../../api/Api';
import QueryTable from './QueryTable';
import BarChart from './graph/BarChart';
import DialogButton from './Dialog';
import Loader from '../../generic-components/Loader';
import { queriesInputBoxSx } from '../../theme';
import {
    CARD_YIELD_ID,
    STATION_YIELD_ID,
    STATION_AND_CARD_YIELD_ID,
    TESTER_LOADER_ID,
    CARD_TEST_DURATION_ID,
    NFF_ID,
    CARD_YIELD_ICON,
    STATION_YIELD_ICON,
    STATION_CARD_YIELD_ICON,
    NFF_ICON,
    BOUNDARIES_ICON,
    BOUNDARIES_ID,
    TESTER_LOAD_ICON,
    CARD_TEST_DURATION_ICON,
} from '../../constants/constants';

const iconsList = {
    [CARD_YIELD_ICON]: SdCardIcon,
    [STATION_YIELD_ICON]: LocalGasStationIcon,
    [STATION_CARD_YIELD_ICON]: EvStationIcon,
    [NFF_ICON]: GppBadIcon,
    [BOUNDARIES_ICON]: FenceIcon,
    [TESTER_LOAD_ICON]: TimelapseIcon,
    [CARD_TEST_DURATION_ICON]: AvTimerIcon,
};

const QueryPage = ({ data }) => {
    const [queryElement, setQueryElement] = useState(data);
    const { id, title, fields, url, icon } = queryElement ?? {};

    const [userInput, setUserInput] = useState({});
    const [loading, setLoading] = useState(false);
    const [showQuery, setShowQuery] = useState(false);
    const [tableData, setTableData] = useState(null);
    const [chartData, setChartData] = useState(null);

    const extractChartData = (records) => {
        let labelProperty;
        let datasets = [];
        let chartData;
        let labels;
        switch (id) {
            case CARD_YIELD_ID:
                labelProperty = 'CardName';
                datasets.push({
                    data: records.map((record) => record['SuccessRatio']),
                    labelString: 'Success Ratio',
                });
                labels = records.map((record) => record[labelProperty]);
                chartData = { datasets, labels };
                break;
            case STATION_YIELD_ID:
                labelProperty = 'Station';
                datasets.push({
                    data: records.map((record) => record['SuccessRatio']),
                    labelString: 'Success Ratio',
                });
                labels = records.map((record) => record[labelProperty]);
                chartData = { datasets, labels };
                break;
            case STATION_AND_CARD_YIELD_ID:
                labelProperty = 'CardName';
                datasets.push({
                    data: records.map((record) => record['SuccessRatio']),
                    labelString: 'Success Ratio',
                });
                labels = records.map((record) => record[labelProperty]);
                chartData = { datasets, labels };
                break;
            case TESTER_LOADER_ID:
                labelProperty = 'Station';
                datasets.push({
                    data: records.map((record) => record['TotalRunTimeHours']),
                    labelString: 'Total Runtime [Hours]',
                });
                datasets.push({
                    data: records.map((record) => record['NumberOfRuns']),
                    labelString: 'Number of Runs',
                });
                labels = records.map((record) => record[labelProperty]);
                chartData = { datasets, labels };
                break;
            case CARD_TEST_DURATION_ID:
                labelProperty = 'Operator';
                datasets.push({
                    data: records.map((record) => record['NetTimeAvg']),
                    labelString: 'Net Time Avg',
                });
                datasets.push({
                    data: records.map((record) => record['TotalTimeAvg']),
                    labelString: 'Total Time Avg',
                });
                labels = records.map((record) => record[labelProperty]);
                chartData = { datasets, labels };
                break;

            case NFF_ID:
                //Graph for false negative by operator
                labelProperty = 'Operator';
                labels = records.map((record) => record[labelProperty]);
                var data = labels.map(
                    (operator) =>
                        records.filter((record) => record.Operator === operator)
                            .length
                );
                datasets.push({
                    data: data,
                    labelString: 'Number of negative failures',
                });

                chartData = { datasets, labels };
                break;
            default:
                break;
        }
        return chartData;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setLoading(true);
        const request = { url, data: userInput };
        const result = await QueryPost(request);
        if (result) {
            setTableData(dataToTable(result));
            extractChartData(result.records);
            showChart() && setChartData(extractChartData(result.records));
        }
    };

    // Convert response data to grid data
    const dataToTable = (data) => {
        const columns = [];
        data.columnNames.map((headerName) =>
            columns.push({
                field: headerName,
                headerName: startCase(headerName),
                flex: 1,
                cellRenderer:
                    id === BOUNDARIES_ID &&
                    headerName === 'Received' &&
                    DialogButton,
            })
        );

        const rows = [];
        data.records.map((record, index) =>
            rows.push({ id: `${index + 1}`, ...record })
        );

        return { rows, columns };
    };

    // Calculate submit button disabling
    const isButtonDisabled = () => {
        if (!fields || keys(userInput).length < fields.length) {
            return true;
        }
        return some(userInput, (field) => field === '');
    };

    const showChart = () =>
        [
            CARD_YIELD_ID,
            STATION_YIELD_ID,
            STATION_AND_CARD_YIELD_ID,
            TESTER_LOADER_ID,
            CARD_TEST_DURATION_ID,
            NFF_ID,
        ].includes(id);

    const renderIcon = () => {
        const Icon = iconsList[icon || 'SdCard'];
        return <Icon />;
    };

    const handleOnChange = (event, field) => {
        setUserInput({
            ...userInput,
            [field.id]:
                field.type === 'number'
                    ? field.id === 'timeInterval'
                        ? 3600 * event.target.valueAsNumber
                        : event.target.valueAsNumber
                    : event.target.value,
        });
    };

    const inputFields = (
        <Box id={`${id}-input-box`} sx={queriesInputBoxSx}>
            {icon && (
                <Avatar sx={{ m: 0, bgcolor: 'secondary.main' }}>
                    {renderIcon()}
                </Avatar>
            )}
            <Typography component='h1' variant='h5'>
                {title}
            </Typography>
            <Box
                key={id}
                component='form'
                noValidate
                onSubmit={handleSubmit}
                sx={{ mt: 1 }}>
                {fields &&
                    fields.map((field, index) => (
                        <TextField
                            key={index}
                            id={`${id}-${field.id}`}
                            required={field.required || false}
                            variant='outlined'
                            margin='normal'
                            fullWidth
                            label={field.label}
                            type={field.type}
                            autoFocus={field.autoFocus || false}
                            onChange={(e) => handleOnChange(e, field)}
                            InputLabelProps={{ shrink: true }}
                        />
                    ))}
                <Button
                    id={`${id}-submit-button`}
                    type='submit'
                    fullWidth
                    variant='contained'
                    disabled={isButtonDisabled()}
                    sx={{ mt: 3, mb: 2 }}>
                    OK
                </Button>
            </Box>
        </Box>
    );

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
        <Box id='query-page-box' component='main' sx={{ flexGrow: 1, py: 8 }}>
            <Container maxWidth={false}>
                <Grid container spacing={3}>
                    <Grid item lg={4} md={6} xl={3} xs={12}>
                        {inputFields}
                    </Grid>
                    {loading && <Loader />}
                    {showQuery && (
                        <Fragment>
                            <Grid item lg={8} md={12} xl={9} xs={12}>
                                <QueryTable
                                    rows={tableData.rows}
                                    columns={tableData.columns}
                                />
                            </Grid>
                            {showChart() && (
                                <Grid item lg={8} md={12} xl={9} xs={12}>
                                    {chartData && tableData.rows.length > 0 && (
                                        <BarChart
                                            datasets={chartData.datasets}
                                            labels={chartData.labels}
                                        />
                                    )}
                                </Grid>
                            )}
                        </Fragment>
                    )}
                </Grid>
            </Container>
        </Box>
    );
};

QueryPage.propTypes = {
    data: PropTypes.shape({
        id: PropTypes.string.isRequired,
        title: PropTypes.string.isRequired,
        url: PropTypes.string.isRequired,
        fields: PropTypes.arrayOf(
            PropTypes.shape({
                id: PropTypes.string.isRequired,
                label: PropTypes.string.isRequired,
                required: PropTypes.bool,
                type: PropTypes.string.isRequired,
                autoFocus: PropTypes.bool,
            })
        ),
        icon: PropTypes.string.isRequired,
    }),
};

export default QueryPage;
