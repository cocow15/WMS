import type { ReactNode } from 'react';   // <- type-only import
import { Navigate } from 'react-router-dom';

type Props = { children: ReactNode };

export default function ProtectedRoute({ children }: Props) {
  const token = localStorage.getItem('access_token');
  return token ? <>{children}</> : <Navigate to="/login" replace />;
}
