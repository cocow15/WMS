import api from '../api/axios';
import { EP } from '../api/endpoints';

export function useAuth() {
  const login = async (username: string, password: string) => {
    const { data } = await api.post(EP.auth.login, { username, password });
    if (data?.data?.token) localStorage.setItem('access_token', data.data.token);
    return data;
  };

  const logout = () => {
    localStorage.removeItem('access_token');
    window.location.href = '/login';
  };

  const isAuthenticated = () => !!localStorage.getItem('access_token');

  return { login, logout, isAuthenticated };
}
