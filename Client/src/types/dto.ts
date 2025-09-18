export type Brand = { brandId: string; name: string };
export type Category = { categoryId: string; name: string };

export type ProductView = {
  productId: string;
  sku: string;
  name: string;
  description?: string | null;
  brandId?: string | null;
  brand?: string | null;
  categoryId?: string | null;
  category?: string | null;
  status: boolean;
  createdAt: string;
};

export type ProductCreateDto = {
  sku: string;
  name: string;
  description?: string | null;
  brandId?: string | null;
  brand?: string | null;
  categoryId?: string | null;
  category?: string | null;
  status: boolean;
};

export type ProductUpdateDto = ProductCreateDto & { productId: string };

export type ProductFilter = {
  guid?: string | null;
  category_id?: string[] | null;
  name?: string | null;
  status?: 'active' | 'inactive' | null;
};

export type ProductListRequest = {
  filter: ProductFilter;
  limit: number;
  page: number;
  order: 'created_at' | 'name';
  sort: 'ASC' | 'DESC';
};
