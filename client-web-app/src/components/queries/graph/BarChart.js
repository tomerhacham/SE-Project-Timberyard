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
import { constant } from 'lodash';

ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

const BarChart = ({datasets,labels}) => {
    //const cardsNames = data.map((record) => record.CardName);
    //const SuccessRatios = data.map((record) => record.SuccessRatio);
    // const data = chartData.data
    // const labels = chartData.labels
    // const labelString = chartData.labelString
    const generateDatasetStruct = (dataset)=>{
        return                     {
            backgroundColor: ['rgba(80, 72, 229, 0.8)'],
            barPercentage: 0.9,
            barThickness: 30,
            borderRadius: 6,
            categoryPercentage: 0.5,
            data: dataset.data,
            label: dataset.labelString,
            maxBarThickness: 50,
        };
    }

    const _datasets = datasets.map((dataset)=>generateDatasetStruct(dataset))
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
