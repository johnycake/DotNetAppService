using DotNetCoreApp1.Models.Types;

namespace DotNetCoreApp1.Models.Interfaces
{
    public interface IRecordRepository
    {
        public Task<IEnumerable<RecordDto>> GetAllRecords();
        public Task<RecordDto?> GetRecord(Guid uuid);
        public Task DeleteRecord(RecordDto recordToDelete);

    }
}
