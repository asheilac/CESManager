using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CESManager.Data;
using CESManager.Dtos.Session;
using CESManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CESManager.Services.SessionService
{
    public class SessionService : ISessionService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetSessionDto>>> AddSession(AddSessionDto newSession)
        {

            ServiceResponse<List<GetSessionDto>> serviceResponse = new ServiceResponse<List<GetSessionDto>>();
            Session session = _mapper.Map<Session>(newSession);
            session.User = await _context.Users.FirstOrDefaultAsync(u => u.Id ==GetUserId());

            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();
            serviceResponse.Data = (_context.Sessions.Where(s => s.User.Id == GetUserId()).Select(s => _mapper.Map<GetSessionDto>(s))).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSessionDto>>> DeleteSession(int id)
        {
            ServiceResponse<List<GetSessionDto>> serviceResponse = new ServiceResponse<List<GetSessionDto>>();
            try
            {
                Session session = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.Id == id && s.User.Id == GetUserId());
                if (session != null)
                {
                    _context.Sessions.Remove(session);
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = (_context.Sessions.Where(s => s.User.Id == GetUserId())
                        .Select(s => _mapper.Map<GetSessionDto>(s))).ToList();
                }
                else 
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Session not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSessionDto>>> GetAllSessions()
        {
            ServiceResponse<List<GetSessionDto>> serviceResponse = new ServiceResponse<List<GetSessionDto>>();
            List<Session> dbSessions = await _context.Sessions.Where(c => c.User.Id == GetUserId()).ToListAsync();
            serviceResponse.Data = (dbSessions.Select(s => _mapper.Map<GetSessionDto>(s))).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSessionDto>> GetSessionById(int id)
        {
            ServiceResponse<GetSessionDto> serviceResponse = new ServiceResponse<GetSessionDto>();
            Session dbSession = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == id & s.User.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetSessionDto>(dbSession);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSessionDto>> UpdateSession(UpdateSessionDto updatedSession)
        {
            ServiceResponse<GetSessionDto> serviceResponse = new ServiceResponse<GetSessionDto>();
            try
            {
                Session session = await _context.Sessions.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == updatedSession.Id);
                if (session.User.Id == GetUserId())
                {
                    session.StartDateTime = updatedSession.StartDateTime;
                    session.EndDateTime = updatedSession.EndDateTime;

                    _context.Sessions.Update(session);
                    await _context.SaveChangesAsync();

                    serviceResponse.Data = _mapper.Map<GetSessionDto>(session);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Session not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}