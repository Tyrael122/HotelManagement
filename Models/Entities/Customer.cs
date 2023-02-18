namespace WebAppHotel.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public Customer()
        {
        }

        public Customer(int id, string name, string email, string phone, string address, string city, string state, string zipCode)
        {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
        }
    }
}
