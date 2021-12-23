import React from 'react';
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

const BarChart = ({ data })=>{
  const records = data.records;
  const cards_names = records.map( r => r.CardName);
  const SuccessRatio = records.map( r => r.SuccessRatio);
    return (
      <Bar
        data={{
          datasets: [
            {
              backgroundColor: ['rgba(80, 72, 229, 0.8)'],
              barPercentage: 0.9,
              barThickness: 30,
              borderRadius: 6,        
              categoryPercentage: 0.5,
              data: SuccessRatio,
              label: 'Success ratio',
              maxBarThickness: 50
            }
          ],
          labels: cards_names
        }}
        height={400}
        width={600}
        options={{
          maintainAspectRatio:false
        }}
      />        
    )
}

export default BarChart;