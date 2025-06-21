namespace Shared.DTOS.Inventory;

public class CreatedSalesOrderSuccessDto
{
    public string DocumentNo { get; }
    public CreatedSalesOrderSuccessDto(string documentNo)
    {
        DocumentNo = documentNo;
    }
}