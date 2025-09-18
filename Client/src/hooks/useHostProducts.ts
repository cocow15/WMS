import { useQuery } from '@tanstack/react-query';
import api from '../api/axios';
import { EP } from '../api/endpoints';

type HostListResponse = {
  app_name: string;
  version: string;
  build: string;
  response: { code: string; status: string; data: any[] };
};

export function useHostList() {
  return useQuery({
    queryKey: ['host', 'list'],
    queryFn: async () => {
      const { data } = await api.get<HostListResponse>(EP.host.productsList);
      return data.response.data ?? [];
    },
  });
}

export function useHostDetail(id?: string) {
  return useQuery({
    queryKey: ['host', 'detail', id],
    enabled: !!id,
    queryFn: async () => {
      const { data } = await api.get<any>(EP.host.productById(id!));
      return data.data; // backend kamu sudah normalisasi ke BaseResponse
    },
  });
}
