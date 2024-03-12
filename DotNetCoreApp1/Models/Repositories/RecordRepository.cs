using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreApp1.Models.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly AppDbContext _appDbContext;

        public RecordRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<RecordDto>> GetAllRecords()
        {
            return await _appDbContext.Records.ToListAsync();
        }

        public async Task<RecordDto?> GetRecord(Guid uuid)
        {
            return await _appDbContext.Records.AsNoTracking().FirstOrDefaultAsync(r => r.RecordId == uuid);
        }

        public async Task DeleteRecord(RecordDto recordToDelete)
        {
            _appDbContext.Remove(recordToDelete);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
