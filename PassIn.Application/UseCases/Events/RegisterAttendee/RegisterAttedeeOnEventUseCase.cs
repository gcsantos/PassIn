using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;
public class RegisterAttedeeOnEventUseCase
{
    private readonly PassInDbContext _dbContext;

    public RegisterAttedeeOnEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisterJson Execute(Guid eventId, RequestRegisterEventJson request)
    {

        Validate(eventId, request);

        var entity = new Infrastructure.Entities.Attendee
        {
            Email = request.Email,
            Name = request.Name,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow,
        };

        _dbContext.Attendees.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisterJson
        {
            Id = entity.Id,
        };

    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var existEtity = _dbContext.Events.Find(eventId);
        if (existEtity is null)
        {
            throw new NotFoundException("ID não encontrado");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("Nome do usuario invalido");
        }

        if (EmailIsValid(request.Email) == false)
        {
            throw new ErrorOnValidationException("Nome do email invalido");
        }

        var attendeeAlreadyRegistered = _dbContext.Attendees.Any(at => at.Email.Equals(request.Email));
        if (attendeeAlreadyRegistered == true) 
        {
            throw new ConflictException("Email já cadastrado");
        }

        var totalParcipantes = _dbContext.Attendees.Count(e => e.Event_Id == eventId);
        if (totalParcipantes == existEtity.Maximum_Attendees)
        {
            throw new ConflictException("Evento lotado!");
        }
    }

    private bool EmailIsValid(string email)
    {
        try
        {
            new MailAddress(email);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
