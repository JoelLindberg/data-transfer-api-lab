using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataTransferApiLab.Models;


public class Audit
{
    [Key]
    public int AuditId { get; private set; }

    [Required]
    [Column(TypeName = "int")]
    public int TransferDataId { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public string TransferName { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(19)")]
    public string Timestamp { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(20)")]
    public string Action { get; set; }

    [Column(TypeName = "int")]
    public int Bytes { get; set; }
}
