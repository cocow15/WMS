import { useState } from "react";
import { Button, Card, Form, Input, Typography, Alert, Divider } from "antd";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { useAuth } from "../../auth/useAuth";

const { Title, Text } = Typography;

export default function LoginPage() {
  const { login } = useAuth();
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  async function onFinish(v: { username: string; password: string }) {
    setErr(null);
    setLoading(true);
    try {
      await login(v.username.trim(), v.password);
      window.location.href = "/";
    } catch (e: any) {
      setErr(e?.message ?? "Login failed. Please check your credentials.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div
      style={{
        minHeight: "100vh",
        display: "grid",
        placeItems: "center",
        background: "#f5f7fa",
        padding: 16,
      }}
    >
      <Card
        style={{ width: 400, boxShadow: "0 8px 24px rgba(0,0,0,.06)" }}
        bordered={false}
      >
        {/* Header */}
        <div style={{ textAlign: "center", marginBottom: 12 }}>
          <Title level={4} style={{ marginBottom: 4 }}>
            WMS Admin
          </Title>
          <Text type="secondary">Sign in to continue</Text>
        </div>

        {/* Optional alert */}
        {err && (
          <Alert
            style={{ marginBottom: 16 }}
            type="error"
            message={err}
            showIcon
          />
        )}

        {/* Form */}
        <Form layout="vertical" onFinish={onFinish} autoComplete="off">
          <Form.Item
            label="Username"
            name="username"
            rules={[{ required: true, message: "Please enter your username" }]}
          >
            <Input
              size="large"
              placeholder="your.username"
              prefix={<UserOutlined />}
              allowClear
            />
          </Form.Item>

          <Form.Item
            label="Password"
            name="password"
            rules={[{ required: true, message: "Please enter your password" }]}
          >
            <Input.Password
              size="large"
              placeholder="••••••••"
              prefix={<LockOutlined />}
            />
          </Form.Item>

          <Button
            type="primary"
            htmlType="submit"
            size="large"
            block
            loading={loading}
          >
            Sign in
          </Button>
        </Form>

        <Divider style={{ margin: "16px 0" }} />

        <div style={{ textAlign: "center" }}>
          <Text type="secondary" style={{ fontSize: 12 }}>
            © {new Date().getFullYear()} WMS – Product Services
          </Text>
        </div>
      </Card>
    </div>
  );
}