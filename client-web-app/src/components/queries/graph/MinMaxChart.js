import React from 'react';
import PropTypes from 'prop-types';
import {
    Chart as ChartJS,
    LinearScale,
    CategoryScale,
    BarElement,
    PointElement,
    LineElement,
    Legend,
    Tooltip,
    ScatterController,
} from 'chart.js';
import { Chart } from 'react-chartjs-2';

ChartJS.register(
    LinearScale,
    CategoryScale,
    BarElement,
    PointElement,
    LineElement,
    Legend,
    Tooltip,
    ScatterController
);

const MinMaxChart = ({ data }) => {
    const points = data.Received;
    const min = data.Min;
    const max = data.Max;

    const labels = points.map(() => '');

    return (
        <Chart
            type='line'
            data={{
                labels,
                datasets: [
                    {
                        type: 'scatter',
                        label: 'Received',
                        radius: 5,
                        hoverRadius: 7,
                        backgroundColor: 'rgba(80, 72, 229, 0.8)',
                        data: points.map((p, i) => ({ x: i, y: p })),
                        borderColor: 'white',
                        borderWidth: 2,
                    },
                    {
                        type: 'line',
                        label: `Min = ${min}`,
                        borderColor: 'rgb(255, 99, 132)',
                        borderWidth: 2,
                        data: points.map((p, i) => ({ x: i, y: min })),
                        pointRadius: 0,
                    },
                    {
                        type: 'line',
                        label: `Max = ${max}`,
                        borderColor: 'rgb(255, 99, 132)',
                        borderWidth: 2,
                        data: points.map((p, i) => ({ x: i, y: max })),
                        pointRadius: 0,
                    },
                ],
            }}
        />
    );
};

MinMaxChart.propTypes = {
    data: PropTypes.shape({
        Max: PropTypes.number.isRequired,
        Min: PropTypes.number.isRequired,
        Received: PropTypes.arrayOf(PropTypes.number).isRequired,
    }),
};

export default MinMaxChart;
