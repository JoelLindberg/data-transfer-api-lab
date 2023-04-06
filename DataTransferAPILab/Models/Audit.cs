namespace DataTransferApiLab.Models;

public class Audit
{
    public int Id { get; private set; }
    public string Date { get; set; }
    public string Event { get; set; }
}
