namespace EventMGT.Models
{
    public class EventUser
    {
        public int Id { get; set; }
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public bool IsRegisteredForMeal { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
