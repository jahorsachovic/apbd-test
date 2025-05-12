using ApbdTest.DTOs;
using Microsoft.Data.SqlClient;
using ApbdTest.Model;
using ApbdTest.DTOs;
using ApbdTest.Exceptions;

namespace ApbdTest.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;

    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }

    public async Task<GetVisitDTO> GetVisitByIdAsync(int visitId)
    {

        await using SqlConnection connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand();

        var query =
            @"SELECT v.date, c.first_name, c.last_name, c.date_of_birth, m.mechanic_id, m.licence_number, s.name, s.base_fee
                      FROM Visit v
                      JOIN Client C on v.client_id = C.client_id
                      JOIN dbo.Mechanic m on m.mechanic_id = v.mechanic_id
                      JOIN dbo.Visit_Service vs on v.visit_id = vs.visit_id
                      JOIN dbo.Service s on vs.service_id = s.service_id
                      WHERE v.visit_id = @visitId;";

        command.Connection = connection;
        command.CommandText = query;

        Console.WriteLine($"Connected to: {connection.Database} {connection.DataSource}");

        Console.WriteLine($"Executing query: {query} with visitId = {visitId}");

        command.Parameters.AddWithValue("@visitId", visitId);
        var reader = await command.ExecuteReaderAsync();

        GetVisitDTO getVisitDto = null;

        List<VisitServiceDTO> visitServices = new();

        while (await reader.ReadAsync())
        {
            if (getVisitDto is null)
            {
                getVisitDto = new GetVisitDTO
                {
                    Date = reader.GetDateTime(0),
                    Client = new ClientVisitDTO
                    {
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        DateOfBirth = reader.GetDateTime(3)
                    },
                    Mechanic = new MechanicVisitDTO
                    {
                        MechanicId = reader.GetInt32(4),
                        LicenseNumber = reader.GetString(5)
                    },
                    VisitServices = visitServices

                };
            }

            visitServices.Add(new VisitServiceDTO
            {
                Name = reader.GetString(6),
                ServiceFee = reader.GetDecimal(7)
            });
        }

        if (getVisitDto is null)
        {
            throw new NotFoundException("Invalid visit id.");
        }

        return getVisitDto;
    }


    public async Task<string> AddNewVisitAsync(AddVisitDTO data)
    {
        await using SqlConnection connection = new SqlConnection(_connectionString);
        
        
        
        return "Insertion successful";
    }
}