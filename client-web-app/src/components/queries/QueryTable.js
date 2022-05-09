import React, { useCallback, useRef } from 'react';
import PropTypes from 'prop-types';
import Button from '@mui/material/Button';
import FileDownloadIcon from '@mui/icons-material/FileDownload';
import { AgGridReact } from 'ag-grid-react';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';

const QueryTable = ({ rows, columns }) => {
    const gridRef = useRef();

    const defaultColDef = {
        resizable: true,
        sortable: true,
        filter: true,
    };

    const onBtnExport = useCallback(() => {
        gridRef.current.api.exportDataAsCsv();
    }, []);

    return (
        <div className='ag-theme-alpine' style={{ height: 400, width: '100%' }}>
            <Button onClick={onBtnExport} startIcon={<FileDownloadIcon />}>
                Export to CSV
            </Button>
            <AgGridReact
                ref={gridRef}
                rowData={rows}
                columnDefs={columns}
                defaultColDef={defaultColDef}></AgGridReact>
        </div>
    );
};

QueryTable.propTypes = {
    rows: PropTypes.array,
    columns: PropTypes.array,
};

export default QueryTable;
