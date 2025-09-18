import { Button, Card, Form, Input, Table, message, Modal, Space, Popconfirm } from "antd";
import { useState } from "react";
import {
  useCategoryCreate,
  useCategoryDelete,
  useCategoryList,
  useCategoryUpdate,
} from "../../hooks/useCategories";
import type { Category } from "../../types/dto";

export default function CategoryList() {
  const { data, isLoading } = useCategoryList();
  const createM = useCategoryCreate();
  const updateM = useCategoryUpdate();
  const delM = useCategoryDelete();

  const [form] = Form.useForm();
  const [editForm] = Form.useForm<{ name: string }>();
  const [pendingCreate, setPendingCreate] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [editing, setEditing] = useState<Category | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [apiMsg, ctx] = message.useMessage();

  // CREATE
  const onCreate = (v: any) => {
    setPendingCreate(true);
    createM.mutate(v, {
      onSuccess: () => {
        apiMsg.success("Category created");
        form.resetFields();
      },
      onError: () => apiMsg.error("Create failed"),
      onSettled: () => setPendingCreate(false),
    });
  };

  // EDIT (open modal)
  const onEdit = (row: Category) => {
    setEditing(row);
    editForm.setFieldsValue({ name: row.name });
    setEditOpen(true);
  };

  // SUBMIT EDIT
  const submitEdit = () => {
    editForm.validateFields().then(({ name }) => {
      if (!editing) return;
      updateM.mutate(
        { categoryId: editing.categoryId, name }, // <-- ID dari row, kirim di body
        {
          onSuccess: () => {
            apiMsg.success("Category updated");
            setEditOpen(false);
            setEditing(null);
          },
          onError: () => apiMsg.error("Update failed"),
        }
      );
    });
  };

  // DELETE (ID dari row, bukan index)
  const handleDelete = (id: string) => {
    setDeletingId(id);
    delM.mutate(id, {
      onSuccess: () => apiMsg.success("Category deleted"),
      onError: () => apiMsg.error("Delete failed"),
      onSettled: () => setDeletingId(null),
    });
  };

  return (
    <Card
      title="Categories"
      extra={
        <Form form={form} layout="inline" onFinish={onCreate}>
          <Form.Item name="name" rules={[{ required: true, message: "Name is required" }]}>
            <Input placeholder="New category name" />
          </Form.Item>
          <Button htmlType="submit" type="primary" loading={pendingCreate}>
            Add
          </Button>
        </Form>
      }
    >
      {ctx}
      <Table
        rowKey="categoryId"
        loading={isLoading}
        dataSource={data ?? []}
        pagination={false}
        columns={[
          { title: "Name", dataIndex: "name" },
          {
            title: "Action",
            width: 220,
            render: (_: any, r: Category) => (
              <Space>
                <Button onClick={() => onEdit(r)}>Edit</Button>
                <Popconfirm
                  title="Delete category?"
                  description={`Kategori "${r.name}" akan dihapus.`}
                  okText="Delete"
                  okButtonProps={{ danger: true }}
                  onConfirm={() => handleDelete(r.categoryId)} // <-- pakai id dari row
                >
                  <Button danger loading={deletingId === r.categoryId}>Delete</Button>
                </Popconfirm>
              </Space>
            ),
          },
        ]}
      />

      <Modal
        title="Edit Category"
        open={editOpen}
        onCancel={() => setEditOpen(false)}
        onOk={submitEdit}
        confirmLoading={updateM.isPending}
        okText="Save"
      >
        <Form form={editForm} layout="vertical">
          <Form.Item
            name="name"
            label="Name"
            rules={[{ required: true, message: "Name is required" }]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
}
