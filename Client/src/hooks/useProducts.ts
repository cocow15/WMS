import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../api/axios';
import { EP } from '../api/endpoints';
import type { BaseResponse } from '../types/base-response';
import type { ProductCreateDto, ProductUpdateDto, ProductListRequest, ProductView } from '../types/dto';

type PageMeta = { page: number; limit: number; total: number; total_pages: number };

export function useProductList(params: ProductListRequest) {
  return useQuery({
    queryKey: ['products', params], // pastikan params stabil (hindari object baru tiap render)
    queryFn: async () => {
      const { data } = await api.post<
        BaseResponse<ProductView[], PageMeta>
      >(EP.productsList, params);

      return {
        rows: data.data ?? [],
        total: data.page?.total ?? 0,
        page: data.page?.page ?? params.page,
      };
    },
    placeholderData: (prev) => prev,
  });
}

export function useProductCreate() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: ProductCreateDto) => {
      const { data } = await api.post<BaseResponse<{ productId: string }>>(EP.products, payload);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }),
  });
}

export function useProductUpdate() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (payload: ProductUpdateDto) => {
      const { data } = await api.put<BaseResponse>(EP.products, payload);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }),
  });
}

export function useProductById(id?: string) {
  return useQuery({
    queryKey: ['product', id],
    enabled: !!id,
    queryFn: async () => {
      const { data } = await api.get<BaseResponse<ProductView>>(`${EP.products}/${id}`);
      return data.data!;
    },
  });
}

export function useProductDelete() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const { data } = await api.delete<BaseResponse>(`${EP.products}/${id}`);
      return data.data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }),
  });
}
