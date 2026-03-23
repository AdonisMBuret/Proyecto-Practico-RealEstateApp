using System.Net;

namespace RealEstateApp.Domain.Exceptions;


public abstract class DomainException : Exception
{
    public abstract HttpStatusCode StatusCode { get; }
    public abstract string ErrorType { get; }

    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}


public class NotFoundException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    public override string ErrorType => "https://tools.ietf.org/html/rfc7231#section-6.5.4";

    public NotFoundException(string resource, object id) 
        : base($"El recurso '{resource}' con ID '{id}' no fue encontrado") { }
    
    public NotFoundException(string message) : base(message) { }
}


public class BusinessValidationException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public override string ErrorType => "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    
    public IDictionary<string, string[]> Errors { get; }

    public BusinessValidationException(IDictionary<string, string[]> errors)
        : base("Se encontraron errores de validación")
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public BusinessValidationException(string field, string error)
        : this(new Dictionary<string, string[]> { { field, new[] { error } } }) { }
}


public class ConflictException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    public override string ErrorType => "https://tools.ietf.org/html/rfc7231#section-6.5.8";

    public ConflictException(string message) : base(message) { }
}


public class ForbiddenException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
    public override string ErrorType => "https://tools.ietf.org/html/rfc7231#section-6.5.3";

    public ForbiddenException(string message) : base(message) { }
}