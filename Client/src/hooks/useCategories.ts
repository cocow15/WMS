import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import api from "../api/axios";
import { EP } from "../api/endpoints";
import type { BaseResponse } from "../types/base-response";
import type { Category } from "../types/dto";

const KEY = ["categories"];

export function useCategoryList() {
  return useQuery({
    queryKey: KEY,
    queryFn: async () => {
      const { data } = await api.get<BaseResponse<Category[]>>(EP.categories);
      return data.data ?? [];
    },
  });
}

export function useCategoryCreate() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: { name: string }) => {
      const { data } = await api.post<BaseResponse>(EP.categories, payload);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: KEY }),
  });
}