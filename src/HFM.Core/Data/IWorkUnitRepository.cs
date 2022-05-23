using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Core.Data;

public class Page<T>
{
    public long CurrentPage { get; set; }

    public long TotalPages { get; set; }

    public long TotalItems { get; set; }

    public long ItemsPerPage { get; set; }

    public IList<T> Items { get; set; }
}

public interface IWorkUnitRepository
{
    Task<long> UpdateAsync(WorkUnitModel workUnitModel);

    Task<int> DeleteAsync(WorkUnitRow row);

    Task<IList<WorkUnitRow>> FetchAsync(WorkUnitQuery query, BonusCalculation bonusCalculation);

    Task<Page<WorkUnitRow>> PageAsync(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation);

    Task<long> CountCompletedAsync(SlotIdentifier slotIdentifier, DateTime? clientStartTime);

    Task<long> CountFailedAsync(SlotIdentifier slotIdentifier, DateTime? clientStartTime);
}
