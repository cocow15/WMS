import { Button, Card, Form, Input, Select, Table, Tag, Space, Modal, Popconfirm, message } from "antd";
import { useMemo, useState } from "react";
import { useProductList, useProductCreate, useProductDelete, useProductUpdate } from "../../hooks/useProducts";
import { useBrandList } from "../../hooks/useBrands";
import { useCategoryList } from "../../hooks/useCategories";
import type { ProductListRequest, ProductCreateDto, ProductUpdateDto, ProductView } from "../../types/dto";

export default function ProductList() {
  const [apiMsg, ctx] = message.useMessage();

  // -------- filter state (memo supaya stable untuk key) ----------
  const [rawParams, setRawParams] = useState<ProductListRequest>({
    filter: { guid: null, category_id: null, name: null, status: null },
    limit: 10,
    page: 1,
    order: "created_at",
    sort: "DESC",
  });
  const params = useMemo(() => rawParams, [rawParams]);

  const { data, isFetching } = useProductList(params);

  // masters
  const { data: brands } = useBrandList();
  const { data: categories } = useCategoryList();

  // forms & modal states
  const [filterForm] = Form.useForm();
  const [editForm] = Form.useForm<ProductCreateDto>();
  const [createOpen, setCreateOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [editing, setEditing] = useState<ProductView | null>(null);

  // mutations
  const createM = useProductCreate();
  const updateM = useProductUpdate();
  const deleteM = useProductDelete();

  // -------- filter submit ----------
  const onSearch = (v: any) => {
    setRawParams((p) => ({
      ...p,
      page: 1,
      filter: {
        ...p.filter,
        name: v.name ?? null,
        status: v.status ?? null,
      },
    }));
  };

  // -------- create ----------
  const openCreate = () => { setCreateOpen(true); editForm.resetFields(); };
  const submitCreate = () => {
    editForm.validateFields().then((vals) => {
      createM.mutate(vals as ProductCreateDto, {
        onSuccess: () => { apiMsg.success("Product created"); setCreateOpen(false); },
        onError: () => apiMsg.error("Create failed"),
      });
    });
  };

  // -------- edit ----------
  const openEdit = (row: ProductView) => {
    setEditing(row);
    setEditOpen(true);
    editForm.setFieldsValue({
      Sku: row.sku as any, // kalau pakai field kecil, sesuaikan form name di bawah
    } as any);
    // mapping field form sesuai DTO create/update:
    editForm.setFieldsValue({
      sku: row.sku,
      name: row.name,
      description: row.description ?? "",
      brandId: row.brandId ?? undefined,
      categoryId: row.categoryId ?? undefined,
      status: row.status,
    } as any);
  };

  const submitEdit = () => {
    editForm.validateFields().then((vals) => {
      if (!editing) return;
      const payload: ProductUpdateDto = {
        ...(vals as ProductCreateDto),
        productId: editing.productId,
      };
      updateM.mutate(payload, {
        onSuccess: () => { apiMsg.success("Product updated"); setEditOpen(false); setEditing(null); },
        onError: () => apiMsg.error("Update failed"),
      });
    });
  };

  // -------- delete ----------
  const onDelete = (id: string) => {
    deleteM.mutate(id, {
      onSuccess: () => apiMsg.success("Product deleted"),
      onError: () => apiMsg.error("Delete failed"),
    });
  };

  return (
    <Card
      title="Products"
      extra={<Button type="primary" onClick={openCreate}>New Product</Button>}
    >
      {ctx}

      <Form form={filterForm} layout="inline" onFinish={onSearch} style={{ marginBottom: 16 }}>
        <Form.Item name="name" label="Name">
          <Input placeholder="Search name" allowClear />
        </Form.Item>
        <Form.Item name="status" label="Status">
          <Select
            allowClear
            options={[
              { value: "active", label: "Active" },
              { value: "inactive", label: "Inactive" },
            ]}
            style={{ width: 160 }}
          />
        </Form.Item>
        <Button htmlType="submit" type="primary">Filter</Button>
      </Form>

      <Table<ProductView>
        rowKey="productId"
        loading={isFetching}
        dataSource={data?.rows ?? []}
        pagination={{
          total: data?.total ?? 0,
          pageSize: params.limit,
          current: params.page,
          onChange: (page, pageSize) => setRawParams((p) => ({ ...p, page, limit: pageSize })),
        }}
        columns={[
          { title: "SKU", dataIndex: "sku" },
          { title: "Name", dataIndex: "name" },
          { title: "Brand", dataIndex: "brand" },
          { title: "Category", dataIndex: "category" },
          {
            title: "Status",
            dataIndex: "status",
            render: (v: boolean) => (v ? <Tag color="green">Active</Tag> : <Tag>Inactive</Tag>),
          },
          {
            title: "Action",
            width: 200,
            render: (_: any, r) => (
              <Space>
                <Button onClick={() => openEdit(r)}>Edit</Button>
                <Popconfirm
                  title="Delete product?"
                  okText="Delete"
                  okButtonProps={{ danger: true }}
                  onConfirm={() => onDelete(r.productId)}
                >
                  <Button danger loading={deleteM.isPending}>Delete</Button>
                </Popconfirm>
              </Space>
            ),
          },
        ]}
      />

      {/* CREATE / EDIT Modal: pakai form yang sama */}
      <Modal
        title={editing ? "Edit Product" : "New Product"}
        open={createOpen || editOpen}
        onCancel={() => { setCreateOpen(false); setEditOpen(false); setEditing(null); }}
        onOk={editing ? submitEdit : submitCreate}
        confirmLoading={createM.isPending || updateM.isPending}
        okText="Save"
      >
        <Form form={editForm} layout="vertical">
          <Form.Item name="sku" label="SKU" rules={[{ required: true }, { max: 50 }]}>
            <Input />
          </Form.Item>
          <Form.Item name="name" label="Name" rules={[{ required: true }, { max: 200 }]}>
            <Input />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item name="brandId" label="Brand">
            <Select
              allowClear
              options={(brands ?? []).map((b: any) => ({ value: b.brandId, label: b.name }))}
              placeholder="Select brand"
            />
          </Form.Item>
          <Form.Item name="categoryId" label="Category">
            <Select
              allowClear
              options={(categories ?? []).map((c: any) => ({ value: c.categoryId, label: c.name }))}
              placeholder="Select category"
            />
          </Form.Item>
          <Form.Item name="status" label="Status" initialValue={true}>
            <Select
              options={[
                { value: true, label: "Active" },
                { value: false, label: "Inactive" },
              ]}
              style={{ width: 160 }}
            />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
}
