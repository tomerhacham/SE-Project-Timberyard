// Convert data returned from server to QueryTable data object
export const dataToTable = (data) => {
    const columns = [];
    data.columnNames.map((headerName) => 
        columns.push({ field: headerName, headerName, width: 150 })
    );

    const rows = [];
    data.records.map((record, index) => 
        rows.push({ id: `${index + 1}`, ...record })
    );

    return { rows, columns };
}