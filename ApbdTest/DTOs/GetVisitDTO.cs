using System.ComponentModel.DataAnnotations;

namespace ApbdTest.DTOs;

public class GetVisitDTO
{
    public DateTime Date { get; set; }
    public ClientVisitDTO Client { get; set; }
    public MechanicVisitDTO Mechanic { get; set; }
    public List<VisitServiceDTO> VisitServices { get; set; }
}

public class ClientVisitDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class MechanicVisitDTO
{
    public int MechanicId { get; set; }
    
    [Required]
    [MaxLength(14)]
    public string LicenseNumber { get; set; }
}

public class VisitServiceDTO
{
    public string Name { get; set; }
    public decimal ServiceFee { get; set; }
}