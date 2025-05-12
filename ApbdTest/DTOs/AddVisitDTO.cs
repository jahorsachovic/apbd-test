using System.ComponentModel.DataAnnotations;
using ApbdTest.DTOs;

namespace ApbdTest.DTOs;


public class AddVisitDTO
{
    [Required]
    public int VisitId { get; set; }
    
    [Required]
    public int ClientId { get; set; }
    
    [Required]
    [StringLength(14)]
    public string MechanicLicenceNumber { get; set; }
    
    [Required]
    public List<AddVisitServiceDTO> Services { get; set; }
}

public class AddVisitServiceDTO
{
    public string ServiceName { get; set; }
    public decimal ServiceFee { get; set; }
}