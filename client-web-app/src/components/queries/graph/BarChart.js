import React from 'react';
import PropTypes from 'prop-types';
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend,
} from 'chart.js';
import { Bar } from 'react-chartjs-2';

ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

const BarChart = ({ datasets, labels }) => {
    const colors = [
        'rgba(80, 72, 229, 0.7)',
        'rgba(237, 16, 245, 0.7)',
        'rgba(255, 205, 86, 0.7)',
        'rgba(75, 192, 192, 0.7)',
        'rgba(54, 162, 235, 0.7)',
        'rgba(153, 102, 255, 0.7)',
        'rgba(201, 203, 207, 0.7)',
        'rgba(255, 99, 132, 0.7)',
    ];
    const generateDatasetStruct = (dataset, color) => {
        return {
            backgroundColor: color,
            barPercentage: 0.9,
            barThickness: 30,
            borderRadius: 6,
            categoryPercentage: 0.5,
            data: dataset.data,
            label: dataset.labelString,
            maxBarThickness: 50,
        };
    };

    const _datasets = datasets.map((dataset, i) =>
        generateDatasetStruct(dataset, colors[i])
    );
    return (
        <Bar
            data={{
                datasets: _datasets,
                labels: labels,
            }}
            height={400}
            width={600}
            options={{
                maintainAspectRatio: false,
            }}
        />
    );
};

BarChart.propTypes = {
    data: PropTypes.arrayOf(
        PropTypes.shape({
            CardName: PropTypes.string.isRequired,
            SuccessRatio: PropTypes.number.isRequired,
        })
    ),
};

export default BarChart;
