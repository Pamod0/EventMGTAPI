namespace EventMGT.DTOs
{
    public class RegistrationStatusResponseDto
    {
        public bool IsRegistered { get; set; }
        public string Message { get; set; }
        public EventUserDto MemberDetails { get; set; }
    }
}
