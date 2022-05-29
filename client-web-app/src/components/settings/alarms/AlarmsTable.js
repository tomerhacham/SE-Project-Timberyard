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
import { SUCCESS_CODE } from '../../../constants/constants';

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

const AlarmsTable = () => {
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
            const response = await EditAlarm(newData);
            if (typeof response === 'object' && response !== null) {
                // Update edited row localy instead of fetching all alarms again
                const rowNode = gridRef.current.api.getRowNode(newData.id);
                rowNode.setData(newData);
                handleCloseDialog();
                setSelectedRows([newData]); // update current selected row data
            } else {
                // TODO: Change
                alert('Error in one or more of the entered details');
            }
        } else {
            // Add new alarm
            const response = await AddNewAlarm(newData);
            if (typeof response === 'object' && response !== null) {
                handleCloseDialog();
                getAlarms();
            } else {
                // TODO: Server should return error as statusText
                alert('Error: could not add alarm');
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
            const response = await RemoveAlarm(id);
            if (response !== SUCCESS_CODE) {
                console.log('DELETE RESPONSE:', response);
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
                    color='primary'
                    variant='contained'
                    onClick={handleAddAlarm}
                    style={{ margin: 10 }}>
                    Add Alarm
                </Button>
                <Button
                    color='primary'
                    variant='contained'
                    onClick={handleEditAlarm}
                    style={{ margin: 10 }}
                    disabled={selectedRows.length === 0}>
                    Edit Selected Alarm
                </Button>
                <Button
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
