using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.RegisterCompany;

public record RegisterCompanyCommand(
    string CompanyName,
    string CompanyCode,
    string CompanyEmail,
    string CompanyPhone,
    string? CompanyAddress,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword,
    string AdminPhoneNumber,
    string CountryName = "India",
    string CountryIsoCode = "IN",
    string StateName = "Maharashtra",
    string CityName = "Mumbai",
    string MainBranchName = "Headquarters",
    string MainBranchCode = "HQ-01",
    string Currency = "USD",
    string TimeZone = "UTC"
) : IRequest<Result<AuthResultDto>>;
