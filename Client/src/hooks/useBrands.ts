import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import api from "../api/axios";
import { EP } from "../api/endpoints";
import type { BaseResponse } from "../types/base-response";

export type Brand = { brandId: string; name: string };

const KEY = ["brands"];

export function useBrandList() {
  return useQuery({
    queryKey: KEY,
    queryFn: async () => {
      const { data } = await api.get<BaseResponse<Brand[]>>(EP.brands);
      return data.data ?? [];
    },
    placeholderData: (prev) => prev,
  });
}

export function useBrandCreate() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: { name: string }) => {
      const { data } = await api.post<BaseResponse>(EP.brands, payload);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: KEY }),
  });
}

/** UPDATE: PUT /api/brands  (ID di body) */
export function useBrandUpdate() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: { brandId: string; name: string }) => {
      const { data } = await api.put<BaseResponse>(EP.brands, payload);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: KEY }),
  });
}

/** DELETE: DELETE /api/brands/{id} */
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
