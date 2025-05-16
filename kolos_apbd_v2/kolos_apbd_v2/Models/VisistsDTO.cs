namespace kolos_apbd_v2.Models;


public class VisitsDTO
{
    public DateTime date { get; set; }   
    public Client client { get; set; }
    public Mechanic mechanic { get; set; }
    public List<Visit_Service> visitServices { get; set; }
}

public class Client
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class Mechanic
{
    public int MechanicId { get; set; }
    public string LicenceNumber { get; set; }
}

public class Visit_Service
{
    public string Name { get; set; }
    public double ServiceFee { get; set; }
}