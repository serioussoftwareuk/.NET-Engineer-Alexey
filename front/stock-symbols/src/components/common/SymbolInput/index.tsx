import React from 'react';
import { debounce } from 'lodash';

import './styles.css';

interface IProps {
  setSymbol: (symbol: string) => void;
}

export const SymbolInput: React.FC<IProps> = ({ setSymbol }: IProps) => {
  const onChange = debounce((symbol: string) => setSymbol(symbol), 1000);

  return (
    <input
      className="textbox"
      onChange={(e) => {
        const { target } = e;
        if (e?.target?.value && e.target.value.length > 1) onChange(target.value);
      }}
    />
  );
};

