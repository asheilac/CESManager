using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CESManager.Data;
using CESManager.Dtos.Session;
using CESManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CESManager.Services.SessionService
{
    public class SessionService : ISessionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SessionService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetSessionDto>>> AddSession(AddSessionDto newSession)
        {
            var serviceResponse = new ServiceResponse<List<GetSessionDto>>();
            try
            {
                var session = _mapper.Map<Session>(newSession);
                session.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == newSession.UserId);
                var result = newSession.Duration;
                if (result > 0)
                {
                    session.StartDateTime = newSession.StartDateTime;
                    session.EndDateTime = newSession.EndDateTime;

                    await _context.Sessions.AddAsync(session);
                    await _context.SaveChangesAsync();

                    serviceResponse.Data = _context.Sessions.Where(s => s.User.Id == session.UserId)
                        .Select(s => _mapper.Map<GetSessionDto>(s)).ToList();
                }
                else
                {
                    serviceResponse.StatusCode = CESManagerStatusCode.NegativeDuration;
                    serviceResponse.Message = "EndDateTime cannot be earlier than StartDateTime.";
                }
            }
            catch
            {
                serviceResponse.StatusCode = CESManagerStatusCode.InternalServerError;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSessionDto>>> DeleteSession(int id, int userId)
        {
            var serviceResponse = new ServiceResponse<List<GetSessionDto>>();
            try
            {
                var session = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.Id == id && s.User.Id == userId);
                if (session != null)
                {
                    _context.Sessions.Remove(session);
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = _context.Sessions.Where(s => s.User.Id == userId)
                        .Select(s => _mapper.Map<GetSessionDto>(s)).ToList();
                }
                else
                {
                    serviceResponse.StatusCode = CESManagerStatusCode.SessionNotFound;
                    serviceResponse.Message = "Could not find session to Delete.";
                }
            }
            catch (Exception e)
            {
                serviceResponse.StatusCode = CESManagerStatusCode.InternalServerError;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSessionDto>>> GetAllSessions(int userId)
        {
            var serviceResponse = new ServiceResponse<List<GetSessionDto>>();
            try
            {
                var dbSessions = await _context.Sessions.Where(c => c.User.Id == userId).ToListAsync();
                if (dbSessions != null)
                {
                    serviceResponse.Data = dbSessions.Select(s => _mapper.Map<GetSessionDto>(s)).ToList();
                }
                else
                {
                    serviceResponse.StatusCode = CESManagerStatusCode.SessionNotFound;
                    serviceResponse.Message = "Could not find sessions.";
                }
            }
            catch
            {
                serviceResponse.StatusCode = CESManagerStatusCode.InternalServerError;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSessionDto>> GetSessionById(int id, int userId)
        {
            var serviceResponse = new ServiceResponse<GetSessionDto>();
            try
            {
                var dbSession = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.Id == id && s.User.Id == userId);
                if (dbSession != null)
                {
                    serviceResponse.Data = _mapper.Map<GetSessionDto>(dbSession);
                    return serviceResponse;
                }

                serviceResponse.StatusCode = CESManagerStatusCode.SessionNotFound;
                serviceResponse.Message = "Could not find session by Id.";
            }
            catch
            {
                serviceResponse.StatusCode = CESManagerStatusCode.InternalServerError;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSessionDto>> UpdateSession(UpdateSessionDto updatedSession)
        {
            var serviceResponse = new ServiceResponse<GetSessionDto>();
            try
            {
                var session = await _context.Sessions.Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == updatedSession.Id);
                if (session != null && session.User.Id == session.UserId)
                {
                    var result = updatedSession.Duration;
                    if (result > 0)
                    {
                        session.StartDateTime = updatedSession.StartDateTime;
                        session.EndDateTime = updatedSession.EndDateTime;

                        _context.Sessions.Update(session);
                        await _context.SaveChangesAsync();

                        serviceResponse.Data = _mapper.Map<GetSessionDto>(session);
                    }
                    else
                    {
                        serviceResponse.StatusCode = CESManagerStatusCode.NegativeDuration;
                        serviceResponse.Message = "EndDateTime cannot be earlier than StartDateTime.";
                    }
                }
                else
                {
                    serviceResponse.StatusCode = CESManagerStatusCode.SessionNotFound;
                    serviceResponse.Message = "Could not find session to update.";
                }
            }
            catch
            {
                serviceResponse.StatusCode = CESManagerStatusCode.InternalServerError;
            }

            return serviceResponse;
        }
    }
}