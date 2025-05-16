using kolos_apbd_v2.Models;

namespace kolos_apbd_v2.Services;

public interface IVisitServices
{
    Task<VisitsDTO> GetVisits(string visitId);
    Task PostVisits(PVisitsDTO visits);
}