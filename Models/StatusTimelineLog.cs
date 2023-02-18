using WebAppHotel.Models.Enums;

namespace WebAppHotel.Models
{
    public class StatusTimelineLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public RoomStatus Status { get; set; }
        public int RoomId { get; set; }

        public StatusTimelineLog(DateTime date, RoomStatus status, int roomId)
        {
            Date = date;
            Status = status;
            RoomId = roomId;
        }

        public StatusTimelineLog() { }
    }
}
