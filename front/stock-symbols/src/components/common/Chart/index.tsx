import React from 'react';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import { PerfomanceModel } from '../../../common/models';

interface IProps {
  data: PerfomanceModel;
  symbol: string;
}

export const Chart: React.FC<IProps> = ({ data, symbol }: IProps) => {
  const prepareLabels = () => {
    const labels = Object.keys(
      data.GivenPerformance.length > data.EtfPerformance.length
        ? data.GivenPerformance
        : data.EtfPerformance
    )
      .map((seconds) => (+seconds * 1000))
      .map((milliseconds) => {
        const date = new Date(milliseconds);
        return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
      });
    return labels;
  };

  const prepareData = (isGiven: boolean) => {
    const values = Object.values(isGiven ? data.GivenPerformance : data.EtfPerformance);
    return values;
  };

  const options = {
    title: {
      text: 'Perfomance'
    },
    series: [
      {
        name: `${symbol}Performance`,
        data: prepareData(true),
      },
      {
        name: 'EtfPerformance',
        data: prepareData(false),
      },
    ],
    xAxis: {
      categories: prepareLabels(),
    },
  }

  return (
    <HighchartsReact
      highcharts={Highcharts}
      options={options}
    />
  );
};

