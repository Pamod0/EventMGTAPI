using AutoMapper;
using EventMGT.Data;
using EventMGT.DTOs;
using EventMGT.Interfaces;
using EventMGT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventMGT.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EventUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEventUserRepository _eventUserService;
        public EventUserController(ApplicationDbContext context, IMapper mapper, IEventUserRepository eventUserService)
        {
            _context = context;
            _mapper = mapper;
            _eventUserService = eventUserService;
        }

        [HttpGet("NIC/{nic}")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> CheckRegistrationStatus(string nic)
        {
            var member = await _context.EventUsers.
                FirstOrDefaultAsync(m => m.NIC == nic);

            if (member == null)
            {
                return Ok(new RegistrationStatusResponseDto
                {
                    IsRegistered = false,
                    Message = "Member not found. Please register for the program."
                });
            }
            return Ok(new RegistrationStatusResponseDto
            {
                IsRegistered = member.IsRegisteredForMeal,
                Message = member.IsRegisteredForMeal
                ? "You have already registered for the program."
                    : "Member not found. Please register for the program.",
                MemberDetails = _mapper.Map<EventUserDto>(member)

            });
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<EventUserDto>> GetAllEventUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchText = "", bool exactMatch = false)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { success = false, message = "Page and PageSize must be greater than 0" });

            var users = await _eventUserService.GetPagedUsersAsync(page, pageSize, searchText, exactMatch);
            return Ok(users);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> RegisterForMeal(RegistrationRequestDto request)
        {

            request.NIC = request.NIC?.Trim() ?? string.Empty;
            request.Name = request.Name?.Trim() ?? string.Empty;
            request.Department = request.Department?.Trim() ?? string.Empty;


            if (string.IsNullOrEmpty(request.NIC))
            {
                return BadRequest("NIC is required");
            }

            var existingMember = await _context.EventUsers
                .FirstOrDefaultAsync(m => m.NIC == request.NIC);

            if (existingMember != null)
            {
                if (!existingMember.IsRegisteredForMeal)
                {
                    existingMember.IsRegisteredForMeal = true;
                    existingMember.RegistrationDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return Ok(new RegistrationStatusResponseDto
                    {
                        IsRegistered = true,
                        Message = "You have successfully registered for the program.",
                        MemberDetails = _mapper.Map<EventUserDto>(existingMember)
                    });
                }
                else
                {
                    return Ok(new RegistrationStatusResponseDto
                    {
                        IsRegistered = true,
                        Message = "You are already registered for the meal program.",
                        MemberDetails = _mapper.Map<EventUserDto>(existingMember)
                    });
                }
            }
            else
            {
                var newMember = _mapper.Map<EventUser>(request);
                _context.EventUsers.Add(newMember);
                await _context.SaveChangesAsync();

                return Ok(new RegistrationStatusResponseDto
                {
                    IsRegistered = true,
                    Message = "You have successfully registered for the program.",
                    MemberDetails = _mapper.Map<EventUserDto>(newMember)
                });
            }
        }

        [HttpPost("Unregister/{nic}")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> UnregisterFromMeal(string nic)
        {
            var member = await _context.EventUsers
                .FirstOrDefaultAsync(m => m.NIC == nic);

            if (member == null)
            {
                return NotFound(new RegistrationStatusResponseDto
                {
                    IsRegistered = false,
                    Message = "Member not found."
                });
            }

            if (member.IsRegisteredForMeal)
            {
                member.IsRegisteredForMeal = false;
                member.RegistrationDate = null;
                await _context.SaveChangesAsync();

                return Ok(new RegistrationStatusResponseDto
                {
                    IsRegistered = false,
                    Message = "Successfully unregistered for the program.",
                    MemberDetails = _mapper.Map<EventUserDto>(member)
                });
            }
            else
            {
                return Ok(new RegistrationStatusResponseDto
                {
                    IsRegistered = false,
                    Message = "You are not registered for the program.",
                    MemberDetails = _mapper.Map<EventUserDto>(member)
                });
            }
        }
    }
}
