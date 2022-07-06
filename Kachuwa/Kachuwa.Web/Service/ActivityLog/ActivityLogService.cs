using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.Web.Model;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public interface IActivityLogService
    {
        CrudService<ActivityLog> CrudService { get; set; }
        Task<bool> CheckPermission(int screenId, ActionType ActionType, int userId);
        Task<ActivityLogResponseModel> Add(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30);
        Task<ActivityLogResponseModel> Update(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30);
        Task<ActivityLogResponseModel> Delete(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30);
        Task<ActivityLogResponseModel> Approve(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30);
        Task<ActivityLogResponseModel> Reject(long logId, int userId, string remarks);
    }
    public class ActivityLogService : IActivityLogService
    {
        private readonly IScreenService _screenService;
        public ActivityLogService(IScreenService screenService)
        {
            _screenService = screenService;

        }
        public CrudService<ActivityLog> CrudService { get; set; } = new CrudService<ActivityLog>();
        public async Task<bool> CheckPermission(int screenId, ActionType ActionType, int userId)
        {
            return true;
        }
        public async Task<ActivityLogResponseModel> Add(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30)
        {
            ActivityLog log = new ActivityLog
            {
                ScreenId = model.ScreenId,
                ActionType = (char)ActionType.New,
                ApprovalLevel = 0,
                CurrentApprovalLevel = 0,
                TransactionData = model.Data,
                TransactionDate = DateTime.Now,
                RefrenceId = model.RefrenceId
            };
            log.AutoFill();
            int id = await CrudService.InsertAsync<int>(db, log, tran, commandTimeout);
            ActivityLogResponseModel r = new ActivityLogResponseModel
            {
                IsSuccess = true,
                AllowActualChange = false,
                ActivityLogId = id,
                RefrenceId = model.RefrenceId
            };
            return r;
        }
        public async Task<ActivityLogResponseModel> Update(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30)
        {
            ActivityLog log = new ActivityLog
            {
                ScreenId = model.ScreenId,
                ActionType = (char)ActionType.Edit,
                ApprovalLevel = 0,
                CurrentApprovalLevel = 0,
                TransactionData = model.Data,
                TransactionDate = DateTime.Now,
                RefrenceId = model.RefrenceId,
                IsCompleted = false,
            };
            log.AutoFill();
            int id = await CrudService.InsertAsync<int>(db, log, tran, commandTimeout);
            ActivityLogResponseModel r = new ActivityLogResponseModel
            {
                IsSuccess = true,
                AllowActualChange = true,
                ActivityLogId = id,
                RefrenceId = model.RefrenceId
            };
            return r;

        }
        public async Task<ActivityLogResponseModel> Delete(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30)
        {
            ActivityLogResponseModel r = new ActivityLogResponseModel();
            ActivityLog log = new ActivityLog
            {
                ScreenId = model.ScreenId,
                ActionType = (char)ActionType.Delete,
                ApprovalLevel = 0,
                CurrentApprovalLevel = 0,
                TransactionData = model.Data,
                TransactionDate = DateTime.Now,
                RefrenceId = model.RefrenceId,
            };
            int id = await CrudService.InsertAsync<int>(db, log, tran, commandTimeout);
            r.IsSuccess = true;
            r.AllowActualChange = true;
            r.ActivityLogId = id;
            r.RefrenceId = model.RefrenceId;
            return r;

        }
        public async Task<ActivityLogResponseModel> Approve(ActivityLogInputModel model, IDbConnection db, IDbTransaction tran, int commandTimeout = 30)
        {
            Screen sc = await _screenService.CrudService.GetAsync(model.ScreenId);
            ActivityLog log = await CrudService.GetAsync(model.ActivityLogId);
            log.IsCompleted = true;
            await CrudService.UpdateAsync(db, log, tran, 30);
            ActivityLogResponseModel r = new ActivityLogResponseModel
            {
                IsSuccess = true,
                AllowActualChange = true,
                ActivityLogId = log.Id,
                Data = log.TransactionData,
                RefrenceId = log.RefrenceId ?? 0,
                ActionType = (ActionType)log.ActionType

            };
            return r;

        }
        public async Task<ActivityLogResponseModel> Reject(long logId, int userId, string remarks)
        {
            ActivityLog log = await CrudService.GetAsync(logId);
            log.IsRejected = true;
            log.RejectedBy = userId;
            log.RejectedOn = DateTime.Now;
            log.RejectedRemarks = remarks;
            log.IsCompleted = true;
            await CrudService.UpdateAsync(log);
            ActivityLogResponseModel r = new ActivityLogResponseModel
            {
                IsSuccess = true,
                AllowActualChange = true,
                ActivityLogId = log.Id,
                RefrenceId = log.RefrenceId ?? 0
            };
            return r;

        }

    }

}