using AutoMapper;
using EventMGT.DTOs;
using EventMGT.Interfaces;
using EventMGT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMGT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventUserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEventUserRepository _eventUserRepository;

        public EventUserController(IMapper mapper, IEventUserRepository eventUserRepository)
        {
            _mapper = mapper;
            _eventUserRepository = eventUserRepository;
        }

        [HttpGet("NIC/{nic}")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> CheckRegistrationStatus(string nic)
        {
            var member = await _eventUserRepository.GetUserByNicAsync(nic);

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
        [Authorize]
        public async Task<ActionResult<PagedResponse<List<EventUserDto>>>> GetAllEventUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchText = "",
            bool exactMatch = false)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { success = false, message = "Page and PageSize must be greater than 0" });

            var users = await _eventUserRepository.GetPagedUsersAsync(page, pageSize, searchText, exactMatch);
            return Ok(users);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> RegisterForMeal(RegistrationRequestDto request)
        {
            // Sanitize input
            request.NIC = request.NIC?.Trim() ?? string.Empty;
            request.Name = request.Name?.Trim() ?? string.Empty;
            request.Department = request.Department?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(request.NIC))
            {
                return BadRequest("NIC is required");
            }

            var existingMember = await _eventUserRepository.GetUserByNicAsync(request.NIC);

            if (existingMember != null)
            {
                if (!existingMember.IsRegisteredForMeal)
                {
                    existingMember.IsRegisteredForMeal = true;
                    existingMember.RegistrationDate = DateTime.Now;

                    await _eventUserRepository.UpdateUserAsync(existingMember);
                    await _eventUserRepository.SaveChangesAsync();

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
                newMember.IsRegisteredForMeal = true;
                newMember.RegistrationDate = DateTime.Now;

                await _eventUserRepository.AddUserAsync(newMember);
                await _eventUserRepository.SaveChangesAsync();

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
            var member = await _eventUserRepository.GetUserByNicAsync(nic);

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

                await _eventUserRepository.UpdateUserAsync(member);
                await _eventUserRepository.SaveChangesAsync();

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