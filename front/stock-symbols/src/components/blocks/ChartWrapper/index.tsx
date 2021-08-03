/* eslint-disable react-hooks/exhaustive-deps */
import React, { useEffect, useState } from 'react';
import { Chart, SymbolInput } from '../../../components/common';
import { PerfomanceModel } from '../../../common/models';
import { PagesEnum } from '../../../common/constants';
import { PerfomanceService } from '../../../services';

import './styles.css';

interface IProps {
  tabId: PagesEnum;
}

export const ChartWrapper: React.FC<IProps> = ({ tabId }: IProps) => {
  const [chartData, setData] = useState<PerfomanceModel>();
  const [symbol, setSymbol] = useState<string>('');

  const loadData = async () => {
    let result;
    switch(tabId) {
      case PagesEnum.PERFOMANCE_DAY:
        result = await PerfomanceService.getPerfomanceByDay(symbol);
        break;
      case PagesEnum.PERFOMANCE_HOUR:
        result = await PerfomanceService.getPerfomanceByHour(symbol);
        break;
      default:
        break;
    }
    setData(result);
  }

  useEffect(() => {
    loadData();
  }, [tabId, symbol]);

  return (
    <div className="chart-wrapper">
      <SymbolInput setSymbol={setSymbol} />
      {
        chartData && (
          <Chart data={chartData} symbol={symbol} />
        )
      }
    </div>
  );
}
