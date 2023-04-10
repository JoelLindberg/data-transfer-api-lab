using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferApiLab.Models;


public class Transfer
{
    [Key]
    public int TransferDataId { get; private set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public string TransferName { get; set; }

    [Required]
    [Column(TypeName = "varchar(500000)")]
    public string TransferData { get; set; }

    [Column(TypeName = "int")]
    public int Bytes { get; private set; }

    public void SetBytes(int bytes)
    {
        Bytes = bytes;
    }
}

public class TransferUploadResponse
{
    public int TransferDataId { get; set; }
    public string TransferName { get; set; }
    public int Bytes { get; set; }
}

public class TransferDownloadResponse
{
    public int TransferDataId { get; set; }
    public string TransferName { get; set; }
    public string TransferData { get; set; }
    public int Bytes { get; set; }
}