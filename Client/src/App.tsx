import { Layout, Menu } from "antd";
import { Link, Outlet, useLocation } from "react-router-dom";

export default function App() {
  const { Header, Sider, Content } = Layout as any;
  const loc = useLocation();
  const key = loc.pathname.split("/")[1] || "dashboard";

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Sider>
        <div style={{ color: "#fff", padding: 16, fontWeight: 600 }}>WMS Admin</div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[key]}
          items={[
            { key: "dashboard", label: <Link to="/">Dashboard</Link> },
            { key: "brands", label: <Link to="/brands">Brands</Link> },
            { key: "categories", label: <Link to="/categories">Categories</Link> },
            { key: "products", label: <Link to="/products">Products</Link> },
            { key: "host", label: <Link to="/host/products">Host Products</Link> },
          ]}
        />
      </Sider>
      <Layout>
        <Header style={{ background: "#fff" }} />
        <Content style={{ margin: 16, background: "#fff", padding: 16 }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}