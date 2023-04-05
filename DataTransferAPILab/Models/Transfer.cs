using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace DataTransferApiLab.Models;


public class Transfer
{
    public int TransferId { get; private set; }
    public string TransferData { get; set; }
}