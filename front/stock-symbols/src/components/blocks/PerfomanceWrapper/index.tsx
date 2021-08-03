import React, { useState } from 'react';
import { Tabs } from '../../common';
import { ChartWrapper } from '../ChartWrapper';
import { PagesEnum } from '../../../common/constants';

import './styles.css';

export const PerfomanceWrapper: React.FC = () => {
  const [currentPage, setPage] = useState<PagesEnum>(PagesEnum.PERFOMANCE_DAY);

  return (
    <div className="perfomance-wrapper">
      <Tabs setTab={(tabId) => setPage(tabId)} />
      <ChartWrapper tabId={currentPage} />
    </div>
  );
}
