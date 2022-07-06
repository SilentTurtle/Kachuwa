using Kachuwa.Data;
using Kachuwa.Data.Crud.Attribute;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Kachuwa.RTC
{
    [Table("ChatContentType")]
    public class ChatContentType
    {
        [Key]
        public int ChatContentTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreInsert]
        public string UpDatedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [IgnoreInsert]
        [AutoFill(false)]
        public bool IsDeleted { get; set; }       
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
    [Table("ChatContent")]
    public class ChatContent
    {
        [Key]
        public long ChatContentId { get; set; }
        public string ChatContentCode { get; set; }
        public string Message { get; set; }
        public string ContentPath { get; set; }
        public string ContentName { get; set; }
        public string ContentSize { get; set; }
        public string SenderConnectionId { get; set; }
        public long SenderId { get; set; }
        public string SenderName { get; set; }
        public long RecieverId { get; set; }
        public string RecieverConnectionId { get; set; }
        public string RecieverName { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public bool IsSendToRoom { get; set; }
        public bool IsActive { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreInsert]
        public string UpDatedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }
       
        [IgnoreInsert]
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
    [Table("Room")]
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreInsert]
        public string UpDatedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }

        [IgnoreInsert]
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }


    public interface IChatService
    {
        CrudService<ChatContentType> ContentTypeService { get; set; }
        CrudService<ChatContent> ContentService { get; set; }
        CrudService<Room> RoomService { get; set; }

        Task<bool> CheckRoomExists(string roomName);
    }
}