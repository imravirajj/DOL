using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string Role = "Buyer"
) : IRequest<Result<AuthResultDto>>;
