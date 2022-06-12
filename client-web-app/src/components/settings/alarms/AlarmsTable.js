import React, { useState, useMemo, useRef, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { Button, Box } from '@mui/material';
import {
    GetAllAlarms,
    EditAlarm,
    AddNewAlarm,
    RemoveAlarm,
} from '../../../api/Api';
import AlarmDialog from './AlarmDialog';
import { MESSAGE } from '../../../constants/constants';

const fieldTypes = {
    0: 'Catalog',
    1: 'Station',
};

const initialValue = {
    name: '',
    field: 0,
    objective: '',
    threshold: 0,
    receivers: [],
};

const AlarmsTable = (props) => {
    const { setMessage } = props;

    const gridRef = useRef();
    const getRowId = useCallback((params) => {
        return params.data.id;
    }, []);

    const [open, setOpen] = useState(false);
    const [formData, setFormData] = useState(initialValue);
    const [selectedRows, setSelectedRows] = useState([]);

    // Grid data states
    const [rowData, setRowData] = useState();
    const [columnDefs] = useState([
        {
            field: 'checkboxBtn',
            headerName: '',
            checkboxSelection: true,
            pinned: 'left',
            filter: false,
            resizable: false,
            minWidth: 50,
            maxWidth: 50,
        },
        {
            field: 'active',
            valueFormatter: (params) => {
                return params.value ? 'true' : 'false';
            },
        },
        { field: 'name', minWidth: 140 },
        {
            field: 'field',
            valueFormatter: (params) => {
                return fieldTypes[params.value];
            },
            singleClickEdit: true,
        },
        { field: 'objective' },
        { field: 'threshold' },
        { field: 'receivers' },
    ]);

    const defaultColDef = useMemo(() => {
        return {
            resizable: true,
            sortable: true,
            filter: true,
        };
    }, []);

    const handleCloseDialog = () => {
        setOpen(false);
        setFormData(initialValue);
    };

    const handleSubmitForm = async (newData) => {
        if (formData.id) {
            // Edit alarm
            const result = await EditAlarm(newData);
            if (result) {
                if (result.status) {
                    // Update edited row locally instead of fetching all alarms again
                    const rowNode = gridRef.current.api.getRowNode(newData.id);
                    rowNode.setData(newData);
                    setSelectedRows([newData]); // update current selected row data
                }
                setMessage({
                    text: result.message,
                    severity: result.status ? MESSAGE.SUCCESS : MESSAGE.ERROR,
                });
                handleCloseDialog();
            }
        } else {
            // Add new alarm
            const result = await AddNewAlarm(newData);
            if (result) {
                setMessage({
                    text: result.message,
                    severity: result.status ? MESSAGE.SUCCESS : MESSAGE.ERROR,
                });
                handleCloseDialog();
                getAlarms();
            }
        }
    };

    const handleEditAlarm = () => {
        setFormData(selectedRows[0]);
        setOpen(true);
    };

    const handleAddAlarm = () => {
        setOpen(true);
    };

    const handleRemoveAlarm = async () => {
        const id = selectedRows[0].id;
        const confirm = window.confirm(
            'Are you sure you want to remove this alarm?'
        );
        if (confirm) {
            const result = await RemoveAlarm({ id });
            if (result) {
                setMessage({
                    text: result.message,
                    severity: result.status ? MESSAGE.SUCCESS : MESSAGE.ERROR,
                });
            }
            getAlarms();
        }
    };

    const onSelectionChanged = () => {
        setSelectedRows(gridRef.current.api.getSelectedRows());
    };

    // Fetch all alarms from server
    const getAlarms = async () => {
        const response = await GetAllAlarms();
        setRowData(response);
    };

    // On grid mount
    const onGridReady = async () => {
        await getAlarms();

        // Resize columns width to fit to content
        const allColumnIds = [];
        gridRef.current.columnApi.getAllColumns().forEach((column) => {
            allColumnIds.push(column.getId());
        });
        gridRef.current.columnApi.autoSizeColumns(allColumnIds);
    };

    return (
        <div id='alarms-div'>
            <div
                className='ag-theme-alpine'
                style={{ height: 400, width: '100%' }}>
                <AgGridReact
                    ref={gridRef}
                    getRowId={getRowId}
                    onGridReady={onGridReady}
                    rowData={rowData}
                    columnDefs={columnDefs}
                    defaultColDef={defaultColDef}
                    rowSelection={'single'}
                    suppressRowClickSelection
                    onSelectionChanged={onSelectionChanged}></AgGridReact>
            </div>
            <Box
                sx={{
                    display: 'flex',
                    justifyContent: 'flex-end',
                    p: 2,
                }}>
                <Button
                    id='add-alarm-button'
                    color='primary'
                    variant='contained'
                    onClick={handleAddAlarm}
                    style={{ margin: 10 }}>
                    Add Alarm
                </Button>
                <Button
                    id='edit-alarm-button'
                    color='primary'
                    variant='contained'
                    onClick={handleEditAlarm}
                    style={{ margin: 10 }}
                    disabled={selectedRows.length === 0}>
                    Edit Selected Alarm
                </Button>
                <Button
                    id='remove-alarm-button'
                    color='primary'
                    variant='contained'
                    onClick={handleRemoveAlarm}
                    style={{ margin: 10 }}
                    disabled={selectedRows.length === 0}>
                    Remove Selected Alarm
                </Button>
            </Box>

            <AlarmDialog
                open={open}
                onClose={handleCloseDialog}
                onSubmit={handleSubmitForm}
                formData={formData}
                fieldTypes={fieldTypes}
            />
        </div>
    );
};

export default AlarmsTable;
