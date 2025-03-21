namespace EventMGT.DTOs
{
    public class RegistrationStatusResponseDto
    {
        public bool IsRegistered { get; set; }
        public string Message { get; set; }
        public MemberDto MemberDetails { get; set; }
    }
}
