import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import ProtectedRoute from '../auth/ProtectedRoute';
import LoginPage from '../pages/Login/LoginPage';
import DashboardPage from '../pages/Dashboard/DashboardPage';
import BrandList from '../pages/Brands/BrandList';
import CategoryList from '../pages/Categories/CategoryList';
import ProductList from '../pages/Products/ProductList';
import HostList from '../pages/Host/HostList';

const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  {
    path: '/',
    element: <ProtectedRoute><DashboardPage/></ProtectedRoute>,
    children: [
      { path: 'brands', element: <BrandList/> },
      { path: 'categories', element: <CategoryList/> },
      { path: 'products', element: <ProductList/> },
      { path: 'host/products', element: <HostList/> },
    ],
  },
]);

export default function AppRouter() { return <RouterProvider router={router} />; }
