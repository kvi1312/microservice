namespace Shared.DTOS.Inventory;

public class SalesOrderDto
{
    public string OrderNo { get; set; } // OrderDocumentNo
    public List<SaleItemDto> SaleItems { get; set; }
}