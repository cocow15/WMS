import { Card, Col, Row, Statistic } from "antd";
import { useBrandList } from "../../hooks/useBrands";
import { useCategoryList } from "../../hooks/useCategories";
import { useProductList } from "../../hooks/useProducts";

export default function DashboardPage() {
  const { data: brands } = useBrandList();
  const { data: categories } = useCategoryList();
  const { data: productsResp } = useProductList({
    filter: { guid: null, category_id: null, name: null, status: null },
    limit: 1,
    page: 1,
    order: "created_at",
    sort: "DESC",
  } as any);

  return (
    <Row gutter={[16, 16]}>
      <Col xs={24} md={8}>
        <Card>
          <Statistic title="Total Brands" value={brands?.length ?? 0} />
        </Card>
      </Col>
      <Col xs={24} md={8}>
        <Card>
          <Statistic title="Total Categories" value={categories?.length ?? 0} />
        </Card>
      </Col>
      <Col xs={24} md={8}>
        <Card>
          <Statistic title="Total Products" value={productsResp?.total ?? 0} />
        </Card>
      </Col>
    </Row>
  );
}
