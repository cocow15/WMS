import { Card, Table, Tag } from "antd";
import { useHostList } from "../../hooks/useHostProducts";

/**
 * Menampilkan data dari HOST (ASMX) yang sudah dinormalisasi backend.
 * Kolom menyesuaikan contoh payload (product_id, sku, name, brand, category, status, created_at).
 */
export default function HostList() {
  const { data, isLoading } = useHostList();

  return (
    <Card title="HOST Products">
      <Table
        rowKey="product_id"
        loading={isLoading}
        dataSource={data ?? []}
        pagination={{ pageSize: 10 }}
        columns={[
          { title: "SKU", dataIndex: "sku" },
          { title: "Name", dataIndex: "name" },
          { title: "Brand", dataIndex: "brand" },
          { title: "Category", dataIndex: "category" },
          {
            title: "Status",
            dataIndex: "status",
            render: (v: boolean) =>
              v ? <Tag color="green">Active</Tag> : <Tag>Inactive</Tag>,
          },
          { title: "Created At", dataIndex: "created_at" },
        ]}
      />
    </Card>
  );
}
