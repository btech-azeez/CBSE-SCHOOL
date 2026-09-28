using CBSESchool.Models;
using CBSESchool.Repositories;

namespace CBSESchool.Services
{
    public class NoticeService : INoticeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NoticeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Notice>> GetAllNoticesAsync(string userRole = "All")
        {
            var notices = await _unitOfWork.Notices.GetAllAsync();
            if (string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return notices.OrderByDescending(n => n.PostDate);
            }

            return notices.Where(n => n.TargetRole == "All" || string.Equals(n.TargetRole, userRole, StringComparison.OrdinalIgnoreCase))
                          .OrderByDescending(n => n.PostDate);
        }

        public async Task<Notice?> GetNoticeByIdAsync(int id)
        {
            return await _unitOfWork.Notices.GetByIdAsync(id);
        }

        public async Task AddNoticeAsync(Notice notice)
        {
            await _unitOfWork.Notices.AddAsync(notice);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateNoticeAsync(Notice notice)
        {
            _unitOfWork.Notices.Update(notice);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteNoticeAsync(int id)
        {
            var notice = await _unitOfWork.Notices.GetByIdAsync(id);
            if (notice != null)
            {
                _unitOfWork.Notices.Remove(notice);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
