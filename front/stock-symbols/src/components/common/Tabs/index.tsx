import React from 'react';
import { PagesEnum } from '../../../common/constants';
import { tabs } from './constants';

import './styles.css';

interface IProps {
  setTab: (tab: PagesEnum) => void;
}

export const Tabs: React.FC<IProps> = ({ setTab }: IProps) => {
  return (
    <div className="tabs-wrapper">
      {
        tabs.map(({ name, tabId }) => (
          <div
            className="tab-item"
            key={tabId}
            onClick={() => setTab(tabId)}
          >
            {name}
          </div>
        ))
      }
    </div>
  );
}
