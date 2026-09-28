using CBSESchool.Models;

namespace CBSESchool.Services
{
    public interface INoticeService
    {
        Task<IEnumerable<Notice>> GetAllNoticesAsync(string userRole = "All");
        Task<Notice?> GetNoticeByIdAsync(int id);
        Task AddNoticeAsync(Notice notice);
        Task UpdateNoticeAsync(Notice notice);
        Task DeleteNoticeAsync(int id);
    }
}
