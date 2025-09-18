import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../api/axios';
import { EP } from '../api/endpoints';
import type { BaseResponse } from '../types/base-response';
import type { Brand } from '../types/dto';

const KEY = ['brands'];

export function useBrandList() {
  return useQuery({
    queryKey: KEY,
    queryFn: async () => {
      const { data } = await api.get<BaseResponse<Brand[]>>(EP.brands);
      return data.data ?? [];
    },
  });
}

export function useBrandCreate() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: { name: string }) => {
      const { data } = await api.post<BaseResponse<{ brandId: string }>>(EP.brands, payload);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: KEY }),
  });
}

export function useBrandUpdate() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: Brand) => {
      const { data } = await api.put<BaseResponse>(' /api/brands', payload);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: KEY }),
  });
}

export function useBrandDelete() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (brandId: string) => {
      const { data } = await api.delete<BaseResponse>(`${EP.brands}/${brandId}`);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: KEY }),
  });
}
