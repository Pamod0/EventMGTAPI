namespace EventMGT.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public bool IsRegisteredForMeal { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
