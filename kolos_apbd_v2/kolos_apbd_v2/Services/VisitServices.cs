using System.Data.SqlClient;
using kolos_apbd_v2.Exceptions;
using kolos_apbd_v2.Models;

namespace kolos_apbd_v2.Services;


public class VisitServices : IVisitServices
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=kolos;Integrated Security=True";


    public async Task<VisitsDTO> GetVisits(string VisitId)
    {
        VisitsDTO? res = null;
        string com = @"SELECT * FROM Visit
        Join Client on Client.client_id = Visit.client_id
        Join Mechanic on Mechanic.mechanic_id = Visit.mechanic_id
        Join Visit_Service on Visit_Service.visit_id = Visit.visit_id
        Join Service on Service.service_id = Visit_Service.service_id
        Where Visit.visit_id = @VisitId";
        using(var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(com, connection))
        {
            await connection.OpenAsync();
            command.Parameters.AddWithValue("@VisitId", int.Parse(VisitId));

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    DateTime date = DateTime.Parse(reader["date"].ToString());
                    Client client = new Client()
                    {
                        FirstName = reader["first_name"].ToString(),
                        LastName = reader["last_name"].ToString(),
                        dateOfBirth = DateTime.Parse(reader["date_of_birth"].ToString()),
                    };

                    Mechanic mechanic = new Mechanic()
                    {
                        MechanicId= (int)reader["mechanic_id"],
                        LicenceNumber = reader["licence_number"].ToString(),
                    };

                    Visit_Service visitService = new Visit_Service()
                    {
                        Name = reader["name"].ToString(),
                        ServiceFee = Convert.ToDouble(reader["service_fee"]),
                    };

                    if (res == null)
                    {
                        res = new VisitsDTO()
                        {
                            date = date,
                            client = client,
                            mechanic = mechanic,
                            visitServices = new List<Visit_Service>()
                        };
                        res.visitServices.Add(visitService);
                        
                    }
                    else
                    {
                        res.visitServices.Add(visitService);
                    }
                }

               
            }
        }
        if (res == null)
        {
            throw new NotFoundException("Visit not found");
        }
        return res;
    }

    public async Task PostVisits(PVisitsDTO visits)
    {
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand("",connection))
        {
            await connection.OpenAsync();
            command.CommandText = @"Select Count(*) from Visit Where visit_id = @VisitId";
            command.Parameters.AddWithValue("@VisitId",visits.visitId);
            
            var ex1 = (int)await command.ExecuteScalarAsync();
            if (ex1 == 0)
            {
                throw new ConflictException("Visit already exists");
            }
            
            command.Parameters.Clear();

            command.CommandText = @"SELECT COUNT(*) FROM Client WHERE client_id = @Clientid";
            command.Parameters.AddWithValue("@Clientid", visits.clientId);
            
            ex1 = (int)await command.ExecuteScalarAsync();
            if (ex1 == 0)
            {
                throw new NotFoundException("Client does not exist");
            }
            
            command.Parameters.Clear(); 
            command.CommandText = @"SELECT COUNT(*) FROM Mechanic WHERE mechanic_id  = (SELECT mechanic_id FROM Mechanic where licence_number = @licenceNumber)";
            command.Parameters.AddWithValue("@licenceNumber", visits.mechanicLicenceNumber);

            if (await command.ExecuteScalarAsync() == DBNull.Value)
            {
                throw new NotFoundException("Mechanic does not exist");
            }
            command.Parameters.Clear();
            
            command.CommandText = @"INSERT INTO Visit VALUES (@visit_id, @client_id, (SELECT mechanic_id FROM Mechanic where licence_number = @licence_number), GETDATE())";
            command.Parameters.AddWithValue("@visit_id", visits.visitId);
            command.Parameters.AddWithValue("@client_id", visits.clientId);
            command.Parameters.AddWithValue("@licenceNumber", visits.mechanicLicenceNumber);
            
            await command.ExecuteNonQueryAsync();
            foreach (var serv in visits.services)
            {
                command.Parameters.Clear();
                command.CommandText = @"Select Count(*) from Service where name = @service_name";
                command.Parameters.AddWithValue("@service_name", serv.Name);

                if (!((int)await command.ExecuteScalarAsync() > 0))
                {
                    throw new NotFoundException("Service does not exist");
                }
                command.Parameters.Clear();
                command.CommandText = @"INSERT INTO Visit_Service (visit_id, service_id, service_fee)
                VALUES(@visitId,(SELECT service_id FROM Service where name = @serviceName),@serviceFee);";

                command.Parameters.AddWithValue("@visitId", visits.visitId);
                command.Parameters.AddWithValue("@serviceName", serv.Name);
                command.Parameters.AddWithValue("@serviceFee", serv.ServiceFee);

                await command.ExecuteNonQueryAsync();
            }
            
        }
    }

    
}