import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'http://localhost:5050',
});

export { axiosInstance };
