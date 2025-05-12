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
    [MaxLength(14)]
    public string mechanicLicenseNumber { get; set; }
    
    [Required]
    public List<VisitServiceDTO> VisitServices { get; set; }
}