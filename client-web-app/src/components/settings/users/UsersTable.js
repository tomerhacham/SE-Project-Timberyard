import React, { useState, useMemo, useRef, useEffect } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { GetAllUsers } from '../../../api/Api';
import { ROLE } from '../../../constants/constants';

const roleTypes = {
    0: 'Regular User',
    1: 'System Admin',
};

const UsersTable = (props) => {
    const gridRef = useRef();

    const { update, setData } = props;
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
        { field: 'email', flex: 1 },
        {
            field: 'role',
            flex: 1,
            valueFormatter: (params) => {
                return roleTypes[params.value];
            },
        },
    ]);

    const defaultColDef = useMemo(() => {
        return {
            resizable: true,
            sortable: true,
            filter: true,
        };
    }, []);

    const onSelectionChanged = () => {
        setSelectedRows(gridRef.current.api.getSelectedRows());
    };

    // Fetch all users from server
    const getUsers = async () => {
        const response = await GetAllUsers();
        setRowData(response);
    };

    // On grid mount
    const onGridReady = async () => {
        await getUsers();
    };

    useEffect(() => {
        if (selectedRows[0]?.email) {
            setData({
                email: selectedRows[0].email,
                role: selectedRows[0].role === 0 ? ROLE.USER : ROLE.ADMIN,
            });
        } else {
            setData(undefined);
        }
    }, [selectedRows, setData]);

    useEffect(() => {
        if (update > 0) {
            getUsers();
        }
    }, [update]);

    return (
        <div id='users-table-div'>
            <div
                className='ag-theme-alpine'
                style={{ height: 400, width: '100%' }}>
                <AgGridReact
                    ref={gridRef}
                    onGridReady={onGridReady}
                    rowData={rowData}
                    columnDefs={columnDefs}
                    defaultColDef={defaultColDef}
                    rowSelection={'single'}
                    suppressRowClickSelection
                    onSelectionChanged={onSelectionChanged}></AgGridReact>
            </div>
        </div>
    );
};

export default UsersTable;
