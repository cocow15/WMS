import { Button, Card, Form, Input, Table, message, Modal } from "antd";
import { useCategoryCreate, useCategoryList } from "../../hooks/useCategories";
import { useState } from "react";

export default function CategoryList() {
  const { data, isLoading } = useCategoryList();
  const createM = useCategoryCreate();
  const [form] = Form.useForm();
  const [apiMsg, ctx] = message.useMessage();
  const [pending, setPending] = useState(false);

  const onCreate = (v: any) => {
    setPending(true);
    createM.mutate(v, {
      onSuccess: () => {
        apiMsg.success("Category created");
        form.resetFields();
      },
      onError: () => apiMsg.error("Create failed"),
      onSettled: () => setPending(false),
    });
  };

  // (opsional) kamu bisa tambah delete di hooks categories dan Modal.confirm seperti BrandList

  return (
    <Card
      title="Categories"
      extra={
        <Form form={form} layout="inline" onFinish={onCreate}>
          <Form.Item name="name" rules={[{ required: true, message: "Name is required" }]}>
            <Input placeholder="New category name" />
          </Form.Item>
          <Button htmlType="submit" type="primary" loading={pending}>
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
        columns={[{ title: "Name", dataIndex: "name" }]}
      />
    </Card>
  );
}
