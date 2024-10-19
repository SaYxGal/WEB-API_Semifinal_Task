using DocumentService.Models.History.DTO;

namespace DocumentService.Services.Histories;

public interface IHistoryService
{
    public Task<List<GetHistoryRecordDTO>> GetAccountHistory(string accountId);

    public Task<GetHistoryRecordDTO> GetHistoryRecord(int id);

    public Task CreateHistoryRecord(AddHistoryRecordDTO dto);

    public Task UpdateHistoryRecord(int id, UpdateHistoryRecordDTO dto);
}
