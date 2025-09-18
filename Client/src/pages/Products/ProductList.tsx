import { Button, Card, Form, Input, Select, Table, Tag } from 'antd';
import { useState } from 'react';
import { useProductList } from '../../hooks/useProducts';

export default function ProductList() {
  const [form] = Form.useForm();
  const [params, setParams] = useState({
    filter: { guid: null, category_id: null, name: null, status: null },
    limit: 10, page: 1, order: 'created_at' as const, sort: 'DESC' as const,
  });

  const { data, isFetching } = useProductList(params);

  const onSearch = (v: any) => {
    setParams((p) => ({
      ...p,
      page: 1,
      filter: {
        ...p.filter,
        name: v.name ?? null,
        status: v.status ?? null,
      },
    }));
  };

  return (
    <Card title="Products">
      <Form form={form} layout="inline" onFinish={onSearch} style={{ marginBottom: 16 }}>
        <Form.Item name="name" label="Name">
          <Input placeholder="Search name" allowClear />
        </Form.Item>
        <Form.Item name="status" label="Status">
          <Select allowClear options={[{value:'active',label:'Active'},{value:'inactive',label:'Inactive'}]} style={{width:160}} />
        </Form.Item>
        <Button htmlType="submit" type="primary">Filter</Button>
      </Form>

      <Table
        rowKey="productId"
        loading={isFetching}
        dataSource={data?.rows ?? []}
        pagination={{
          total: data?.total ?? 0,
          pageSize: params.limit,
          current: params.page,
          onChange: (page, pageSize) => setParams((p)=>({ ...p, page, limit: pageSize })),
        }}
        columns={[
          { title: 'SKU', dataIndex: 'sku' },
          { title: 'Name', dataIndex: 'name' },
          { title: 'Brand', dataIndex: 'brand' },
          { title: 'Category', dataIndex: 'category' },
          {
            title: 'Status',
            dataIndex: 'status',
            render: (v:boolean) => v ? <Tag color="green">Active</Tag> : <Tag>Inactive</Tag>
          },
        ]}
      />
    </Card>
  );
}
