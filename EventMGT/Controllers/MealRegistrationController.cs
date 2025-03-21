using AutoMapper;
using EventMGT.Data;
using EventMGT.DTOs;
using EventMGT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventMGT.Controllers
{
    public class MealRegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MealRegistrationController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("check/{nic}")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> CheckRegistrationStatus(string nic)
        {
            var member = await _context.Members.
                FirstOrDefaultAsync(m => m.NIC == nic);

            if (member == null)
            {
                return Ok(new RegistrationStatusResponseDto
                {
                    IsRegistered = false,
                    Message = "Member not found. You can register for the meal program."
                });
            }
            return Ok(new RegistrationStatusResponseDto
            {
                IsRegistered = member.IsRegisteredForMeal,
                Message = member.IsRegisteredForMeal
                ? "You have already registered for the meal program."
                    : "You are not registered for the meal program. You can register now.",
                MemberDetails = _mapper.Map<MemberDto>(member)

            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> RegisterForMeal(RegistrationRequestDto request)
        {

            request.NIC = request.NIC?.Trim() ?? string.Empty;
            request.Name = request.Name?.Trim() ?? string.Empty;
            request.Department = request.Department?.Trim() ?? string.Empty;


            if (string.IsNullOrEmpty(request.NIC))
            {
                return BadRequest("NIC is required");
            }

            var existingMember = await _context.Members
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
                        Message = "You have successfully registered for the meal program.",
                        MemberDetails = _mapper.Map<MemberDto>(existingMember)
                    });
                }
                else
                {
                    return Ok(new RegistrationStatusResponseDto
                    {
                        IsRegistered = true,
                        Message = "You are already registered for the meal program.",
                        MemberDetails = _mapper.Map<MemberDto>(existingMember)
                    });
                }
            }
            else
            {
                var newMember = _mapper.Map<Member>(request);
                _context.Members.Add(newMember);
                await _context.SaveChangesAsync();

                return Ok(new RegistrationStatusResponseDto
                {
                    IsRegistered = true,
                    Message = "You have successfully registered for the meal program.",
                    MemberDetails = _mapper.Map<MemberDto>(newMember)
                });
            }
        }

        [HttpPost("unregister/{nic}")]
        public async Task<ActionResult<RegistrationStatusResponseDto>> UnregisterFromMeal(string nic)
        {
            var member = await _context.Members
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
                    Message = "Successfully unregistered from the meal program.",
                    MemberDetails = _mapper.Map<MemberDto>(member)
                });
            }
            else
            {
                return Ok(new RegistrationStatusResponseDto
                {
                    IsRegistered = false,
                    Message = "You are not registered for the meal program.",
                    MemberDetails = _mapper.Map<MemberDto>(member)
                });
            }
        }
    }
}
