using ApbdTest.Model;
using ApbdTest.DTOs;

namespace ApbdTest.Services;

public interface IDbService
{
    Task<GetVisitDTO> GetVisitByIdAsync(int customerId);
    Task<string> AddNewVisitAsync(AddVisitDTO data);
}