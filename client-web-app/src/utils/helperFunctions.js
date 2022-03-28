// Convert data returned from server to QueryTable data object
export const dataToTable = (data) => {
    let columns = [];
    data.columnNames.map((headerName) => columns.push({ field: headerName, headerName, width: 150 }));

    let rows = [];
    data.records.map((record, index) => rows.push({ id: `${index + 1}`, ...record }))

    return { rows, columns };
}