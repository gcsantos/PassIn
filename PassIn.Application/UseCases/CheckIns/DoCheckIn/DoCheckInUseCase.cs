using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;


namespace PassIn.Application.UseCases.CheckIns.DoCheckIn;
public class DoCheckInUseCase
{
    private readonly PassInDbContext _dbContext;

    public DoCheckInUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisterJson Execute(Guid attendeeId)
    {
        Validate(attendeeId);

        var entity = new CheckIn
        {
            Attendee_Id = attendeeId,
            Created_at = DateTime.UtcNow,
        };

        _dbContext.CheckIns.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisterJson
        {
            Id = entity.Id
        };
    }
    private void Validate(Guid attendeeId)
    {
        var existeAttendedd = _dbContext.Attendees.Any(at => at.Id == attendeeId);
        if (existeAttendedd == false)
        {
            throw new NotFoundException("Não existe esse participante em evento");
        }

        var existeCheckIn = _dbContext.CheckIns.Any(ch => ch.Attendee_Id == attendeeId);
        if (existeCheckIn)
        {
            throw new ConflictException("Participante ja fez checkIn nesse evento");
        }
    }
}

    
