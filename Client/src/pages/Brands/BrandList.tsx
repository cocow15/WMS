import { Button, Card, Form, Input, Space, Table, message, Modal } from "antd";
import { useBrandCreate, useBrandDelete, useBrandList } from "../../hooks/useBrands";
import { useState } from "react";

export default function BrandList() {
  const { data, isLoading } = useBrandList();
  const createM = useBrandCreate();
  const delM = useBrandDelete();
  const [form] = Form.useForm();
  const [apiMsg, ctx] = message.useMessage();
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const onCreate = (v: any) => {
    createM.mutate(v, {
      onSuccess: () => {
        apiMsg.success("Brand created");
        form.resetFields();
      },
      onError: () => apiMsg.error("Create failed"),
    });
  };

  const confirmDelete = (id: string) => {
    Modal.confirm({
      title: "Delete brand?",
      content: "Brand akan dihapus permanen.",
      okText: "Delete",
      okButtonProps: { danger: true },
      onOk: () => {
        setDeletingId(id);
        delM.mutate(id, {
          onSuccess: () => apiMsg.success("Brand deleted"),
          onError: () => apiMsg.error("Delete failed"),
          onSettled: () => setDeletingId(null),
        });
      },
    });
  };

  return (
    <Card
      title="Brands"
      extra={
        <Form form={form} layout="inline" onFinish={onCreate}>
          <Form.Item name="name" rules={[{ required: true, message: "Name is required" }]}>
            <Input placeholder="New brand name" />
          </Form.Item>
          <Button htmlType="submit" type="primary" loading={createM.isPending}>
            Add
          </Button>
        </Form>
      }
    >
      {ctx}
      <Table
        rowKey="brandId"
        loading={isLoading}
        dataSource={data ?? []}
        pagination={false}
        columns={[
          { title: "Name", dataIndex: "name" },
          {
            title: "Action",
            width: 140,
            render: (_: any, r: any) => (
              <Space>
                <Button
                  danger
                  loading={deletingId === r.brandId}
                  onClick={() => confirmDelete(r.brandId)}
                >
                  Delete
                </Button>
              </Space>
            ),
          },
        ]}
      />
    </Card>
  );
}
