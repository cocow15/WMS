export const EP = {
  auth: {
    register: '/api/auth/register',
    login: '/api/auth/login',
  },
  brands: '/api/brands',
  categories: '/api/categories',
  products: '/api/products',
  productsList: '/api/products/list',
  host: {
    loginAndSave: '/api/external/login-and-save',
    products: '/api/host/products',
    productsList: '/api/host/products/list',
    productById: (id: string) => `/api/host/products/${id}`,
  },
};