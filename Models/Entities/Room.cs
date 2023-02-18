using WebAppHotel.Models.Enums;

namespace WebAppHotel.Models.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public int StatusTimelineId { get; set; }
        public double DailyPrice { get; set; }
        public int MaxPeople { get; set; }
        public RoomStatus Status { get; set; }
        

        public Room(int id, int maxPeople, double dailyPrice, RoomStatus status)
        {
            Id = id;
            MaxPeople = maxPeople;
            DailyPrice = dailyPrice;
            Status = status;
            StatusTimelineId = id;
        }

        public Room() { }
    }
}
