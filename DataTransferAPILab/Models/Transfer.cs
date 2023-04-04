using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace DataTransferApiLab.Models;


public class Transfer
{
    public int TransferId { get; set; }
    public string TransferData { get; set; }
}