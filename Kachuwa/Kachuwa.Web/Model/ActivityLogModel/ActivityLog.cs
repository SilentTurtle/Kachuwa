using Kachuwa.Data.Crud.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web.Model
{
    public class ActivityLog
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public int ScreenId { get; set; }
        [Required]//should be from ActionType enum
        public char ActionType { get; set; }
        [Required]
        public string TransactionData { get; set; }
        [Required]
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime TransactionDate { get; set; }
        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public int AddedBy { get; set; }
        [Required]
        public int ApprovalLevel { get; set; }
        [Required]

        public int CurrentApprovalLevel { get; set; }

        public int? ApprovedByLevelOne { get; set; }

        public DateTime? ApprovedOnLevelOne { get; set; }

        public int? ApprovedByLevelTwo { get; set; }

        public DateTime? ApprovedOnLevelTwo { get; set; }

        public int? ApprovedByLevelThree { get; set; }

        public DateTime? ApprovedOnLevelThree { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public int RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string RejectedRemarks { get; set; }
        public bool IsCompleted { get; set; }

        public bool IsObsolate { get; set; }

        public long? RefrenceId { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
        [IgnoreAll]
        public string PreviousData { get; set; }//assign to this when comparision is required with previous version of record
        [IgnoreAll]
        public Screen Screen { get; set; }
        [IgnoreAll]
        public string ScreenName { get; set; }
        [IgnoreAll]
        public string UserName { get; set; }

    }

}
