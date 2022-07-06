using Kachuwa.Data;
using System.Threading.Tasks;

namespace Kachuwa.RTC
{
    public class ChatService : IChatService
    {
        public CrudService<ChatContentType> ContentTypeService { get; set; } = new CrudService<ChatContentType>();
        public CrudService<ChatContent> ContentService { get; set; } = new CrudService<ChatContent>();
        public CrudService<Room> RoomService { get; set; } = new CrudService<Room>();

        public async Task<bool> CheckRoomExists(string roomName)
        {
           var room=await RoomService.GetAsync("Where Name=@Name and IsActive=@IsActive and IsDeleted=@IsDeleted", new { Name = roomName, IsActive = true, IsDeleted = false });
            return room == null ? false : true;
        }
    }
}