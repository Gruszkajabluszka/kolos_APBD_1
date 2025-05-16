using Microsoft.Build.Framework;

namespace kolos_apbd_v2.Models;

public class PVisitsDTO
{
    [Required] public int visitId { get; set; }
    [Required] public int clientId { get; set; }
    [Required] public string mechanicLicenceNumber { get; set; }
    [Required] public List<Visit_Service> services { get; set; } = [];
}