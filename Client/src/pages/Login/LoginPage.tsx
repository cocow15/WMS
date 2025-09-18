import { Button, Card, Form, Input, Typography } from 'antd';
import { useAuth } from '../../auth/useAuth';

export default function LoginPage() {
  const { login } = useAuth();

  const onFinish = async (v: { username: string; password: string }) => {
    await login(v.username, v.password);
    window.location.href = '/';
  };

  return (
    <div className="h-screen flex items-center justify-center bg-[#f5f7fa]">
      <Card style={{ width: 380 }}>
        <Typography.Title level={4} style={{ marginBottom: 24 }}>
          WMS Admin
        </Typography.Title>
        <Form layout="vertical" onFinish={onFinish}>
          <Form.Item label="Username" name="username" rules={[{ required: true }]}>
            <Input placeholder="username" />
          </Form.Item>
          <Form.Item label="Password" name="password" rules={[{ required: true }]}>
            <Input.Password placeholder="••••••••" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block>
            Login
          </Button>
        </Form>
      </Card>
    </div>
  );
}
