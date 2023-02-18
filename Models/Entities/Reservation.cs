using WebAppHotel.Models.Enums;

namespace WebAppHotel.Models.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public double Price { get; set; }
        public ReservationStatus Status { get; set; }

        public Reservation(int id, int customerId, int roomId, DateTime checkIn, DateTime checkOut, double price, ReservationStatus status)
        {
            if (IsDateValid(checkIn, checkOut))
            {   
                Id = id;
                CustomerId = customerId;
                RoomId = roomId;
                CheckIn = checkIn;
                CheckOut = checkOut;
                Price = price;
                Status = status;
            }
        }

        public Reservation() { }

        public static bool IsDateValid(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut) 
            {
                throw new ArgumentException("Check-in date must be before check-out date.");
            }
            if (checkIn < DateTime.Now || checkOut < DateTime.Now)
            {
                throw new ArgumentException("Check-in and check-out dates must be in the future.");
            }
            if (checkOut.Subtract(checkIn).Hours < 8)
            {
                throw new ArgumentException("Reservation must be at least 8 hours long.");
            }
            return true;
        }
    }
}
