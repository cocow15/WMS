import { Button, Card, Form, Input, Space, Table, message, Modal, Popconfirm } from "antd";
import { useState } from "react";
import {
  useBrandCreate,
  useBrandDelete,
  useBrandList,
  useBrandUpdate,
  type Brand,
} from "../../hooks/useBrands";

export default function BrandList() {
  const { data, isLoading } = useBrandList();
  const createM = useBrandCreate();
  const updateM = useBrandUpdate();
  const delM = useBrandDelete();

  const [form] = Form.useForm();
  const [editForm] = Form.useForm<{ name: string }>();
  const [editOpen, setEditOpen] = useState(false);
  const [editingBrand, setEditingBrand] = useState<Brand | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [apiMsg, ctx] = message.useMessage();

  const onCreate = (v: any) => {
    createM.mutate(v, {
      onSuccess: () => { apiMsg.success("Brand created"); form.resetFields(); },
      onError: () => apiMsg.error("Create failed"),
    });
  };

  const onEdit = (row: Brand) => {
    setEditingBrand(row);
    editForm.setFieldsValue({ name: row.name });
    setEditOpen(true);
  };

  const submitEdit = () => {
    editForm.validateFields().then(({ name }) => {
      if (!editingBrand) return;
      updateM.mutate(
        { brandId: editingBrand.brandId, name },
        {
          onSuccess: () => { apiMsg.success("Brand updated"); setEditOpen(false); setEditingBrand(null); },
          onError: () => apiMsg.error("Update failed"),
        }
      );
    });
  };

  const handleDelete = (id: string) => {
    console.log("delete click", id); // debug: pastikan event terpanggil
    setDeletingId(id);
    delM.mutate(id, {
      onSuccess: () => apiMsg.success("Brand deleted"),
      onError: () => apiMsg.error("Delete failed"),
      onSettled: () => setDeletingId(null),
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
          <Button htmlType="submit" type="primary" loading={createM.isPending}>Add</Button>
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
            width: 220,
            render: (_: any, r: Brand) => (
              <Space>
                <Button onClick={() => onEdit(r)}>Edit</Button>
                <Popconfirm
                  title="Delete brand?"
                  description={`Brand "${r.name}" akan dihapus.`}
                  okText="Delete"
                  okButtonProps={{ danger: true }}
                  onConfirm={() => handleDelete(r.brandId)} // <-- ID dari record
                >
                  <Button danger loading={deletingId === r.brandId}>Delete</Button>
                </Popconfirm>
              </Space>
            ),
          },
        ]}
      />

      <Modal
        title="Edit Brand"
        open={editOpen}
        onCancel={() => setEditOpen(false)}
        onOk={submitEdit}
        confirmLoading={updateM.isPending}
        okText="Save"
      >
        <Form form={editForm} layout="vertical">
          <Form.Item name="name" label="Name" rules={[{ required: true, message: "Name is required" }]}>
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
}
