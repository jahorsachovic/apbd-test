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
        _connectionString = configuration.GetConnectionString("Default");
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
        await connection.OpenAsync();
        //await using SqlCommand command = new SqlCommand();

        //Validate if the Visit with this id already exists
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"
                                    SELECT 1 FROM Visit WHERE visit_id = @VisitId;
                                    ";
            command.Parameters.AddWithValue("@VisitId", data.VisitId);

            var result = await command.ExecuteScalarAsync();

            if (result != null)
            {
                throw new NotFoundException("Invalid visit Id");
            }
        }

        //Validate if the Client exists
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"
                                    SELECT 1 FROM Client WHERE client_id = @ClientId; 
                                    ";
            command.Parameters.AddWithValue("@ClientId", data.ClientId);

            var result = command.ExecuteScalarAsync();

            if (result == null)
            {
                throw new NotFoundException("Invalid client id");
            }
        }

        //Validate if mechanic with such license number exists
        int NewVisitMechanicId;
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"
                                    SELECT 1 FROM Mechanic WHERE licence_number = @MechanicLicenseNumber
                                    ";
            command.Parameters.AddWithValue("@MechanicLicenseNumber", data.MechanicLicenceNumber);

            var reader = await command.ExecuteReaderAsync();
            
            
            
            if (!await reader.ReadAsync())
            {
                throw new NotFoundException("Invalid mechanic's license number");
            }

            NewVisitMechanicId = reader.GetInt32(0);

        }

        // Validate if provided Services exist
        foreach (var service in data.Services)
        {
            await using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"
                                        SELECT 1 FROM Service WHERE name = @ServiceName;
                                        ";
                command.Parameters.AddWithValue("@ServiceName", service.ServiceName);

                var result = await command.ExecuteScalarAsync();

                if (result ==
                    null)
                {
                    throw new NotFoundException($"Invalid service name: {service.ServiceName}");
                }
            }
        }
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            
        }

        //Add a new visit table entry
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"INSERT INTO Visit (visit_id, client_id, mechanic_id, date)
                                    VALUES (@VisitId, @ClientId, @MechanicId, GETDATE());
                                    ";
            command.Parameters.AddWithValue("@VisitId", data.VisitId);
            command.Parameters.AddWithValue("@ClientId", data.ClientId);
            command.Parameters.AddWithValue("@MechanicId", NewVisitMechanicId);

            return "Insertion successful";
            
        }
    }
}