import React from "react";
import ReactDOM from "react-dom/client";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import ProtectedRoute from "./auth/ProtectedRoute";
import App from "./App"; // <- shell di dalam Router
import LoginPage from "./pages/Login/LoginPage";
import DashboardPage from "./pages/Dashboard/DashboardPage";
import BrandList from "./pages/Brands/BrandList";
import CategoryList from "./pages/Categories/CategoryList";
import ProductList from "./pages/Products/ProductList";
import HostList from "./pages/Host/HostList";
import "antd/dist/reset.css";

const qc = new QueryClient();

const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <App />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <DashboardPage /> },
      { path: "brands", element: <BrandList /> },
      { path: "categories", element: <CategoryList /> },
      { path: "products", element: <ProductList /> },
      { path: "host/products", element: <HostList /> },
    ],
  },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <QueryClientProvider client={qc}>
      <RouterProvider router={router} />
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  </React.StrictMode>
);
