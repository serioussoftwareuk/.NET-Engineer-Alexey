import { axiosInstance } from '../utils';
import { PerfomanceModel } from '../common/models';

export class PerfomanceService {
  public static async getPerfomanceByDay(symbol: string): Promise<PerfomanceModel> {
    const result = await axiosInstance.get<PerfomanceModel>(
      'StockSymbol/perfCompByDay',
      {
        params: {
          symbol,
        }
      }
    )
      .then((result) => result.data);

    return result;
  }

  public static async getPerfomanceByHour(symbol: string): Promise<PerfomanceModel> {
    const result = await axiosInstance.get<PerfomanceModel>(
      'StockSymbol/perfCompByHour',
      {
        params: {
          symbol,
        }
      }
    )
      .then((result) => result.data);

    return result;
  }
}
