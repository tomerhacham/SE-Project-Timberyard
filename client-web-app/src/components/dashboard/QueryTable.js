import React from 'react'
import {
    DataGrid,
    GridToolbarContainer,
    GridToolbarExport,
    gridClasses,
} from '@mui/x-data-grid';

// const rows = [
//   { id: 1, col1: 'Hello', col2: 'World' },
//   { id: 2, col1: 'DataGridPro', col2: 'is Awesome' },
//   { id: 3, col1: 'MUI', col2: 'is Amazing' },
// ];

// const columns = [
//   { field: 'col1', headerName: 'Column 1', width: 150 },
//   { field: 'col2', headerName: 'Column 2', width: 150 },
// ];

function CustomToolbar() {
    return (
      <GridToolbarContainer className={gridClasses.toolbarContainer}>
        <GridToolbarExport />
      </GridToolbarContainer>
    );
}

const QueryTable = ({ rows, columns }) => {
    return (
        <div style={{ height: '100%', width: '100%' }}>
            <DataGrid
                rows={rows} 
                columns={columns}
                components={{
                Toolbar: CustomToolbar,
                }}
            />
        </div>
    )
}

export default QueryTable;
