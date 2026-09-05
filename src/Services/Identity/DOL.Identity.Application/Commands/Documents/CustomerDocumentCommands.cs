using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Documents;

// ── Upload Document ─────────────────────────────────────────
public record UploadDocumentCommand(
    Guid CompanyId,
    Guid UserId,
    DocumentType DocumentType,
    string DocumentNumber,
    string FileUrl,
    string FileName,
    long FileSizeBytes,
    Guid? OrderId = null) : IRequest<Result<Guid>>;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DocumentNumber).NotEmpty();
        RuleFor(x => x.FileUrl).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.FileSizeBytes).GreaterThan(0);
    }
}

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public UploadDocumentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = new CustomerDocument(
            request.CompanyId,
            request.UserId,
            request.DocumentType,
            request.DocumentNumber,
            request.FileUrl,
            request.FileName,
            request.FileSizeBytes,
            request.OrderId);

        _context.CustomerDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(doc.Id);
    }
}

// ── Verify / Reject Document ────────────────────────────────
public record VerifyDocumentCommand(
    Guid Id,
    Guid StaffId,
    bool Approve,
    string? RejectionReason = null) : IRequest<Result<bool>>;

public class VerifyDocumentCommandHandler : IRequestHandler<VerifyDocumentCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public VerifyDocumentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(VerifyDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = await _context.CustomerDocuments.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (doc == null) return Result<bool>.Failure("Document not found.");

        if (request.Approve)
        {
            doc.Verify(request.StaffId);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                return Result<bool>.Failure("Rejection reason is required when rejecting a document.");
            doc.Reject(request.StaffId, request.RejectionReason);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Delete Document ─────────────────────────────────────────
public record DeleteDocumentCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public DeleteDocumentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = await _context.CustomerDocuments.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (doc == null) return Result<bool>.Failure("Document not found.");

        _context.CustomerDocuments.Remove(doc);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetDocumentsQuery(Guid? UserId = null, Guid? OrderId = null) : IRequest<Result<List<CustomerDocumentDto>>>;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, Result<List<CustomerDocumentDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetDocumentsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CustomerDocumentDto>>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CustomerDocuments.AsNoTracking();

        if (request.UserId.HasValue)
        {
            query = query.Where(d => d.UserId == request.UserId.Value);
        }

        if (request.OrderId.HasValue)
        {
            query = query.Where(d => d.OrderId == request.OrderId.Value);
        }

        var list = await query
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new CustomerDocumentDto(
                d.Id,
                d.CompanyId,
                d.UserId,
                d.OrderId,
                d.DocumentType,
                d.DocumentNumber,
                d.FileUrl,
                d.FileName,
                d.FileSizeBytes,
                d.VerificationStatus,
                d.VerifiedByStaffId,
                d.VerifiedAt,
                d.RejectionReason,
                d.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<CustomerDocumentDto>>.Success(list);
    }
}

public record GetDocumentByIdQuery(Guid Id) : IRequest<Result<CustomerDocumentDto>>;

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<CustomerDocumentDto>>
{
    private readonly IIdentityDbContext _context;

    public GetDocumentByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CustomerDocumentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _context.CustomerDocuments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (d == null) return Result<CustomerDocumentDto>.Failure("Document not found.");

        return Result<CustomerDocumentDto>.Success(new CustomerDocumentDto(
            d.Id,
            d.CompanyId,
            d.UserId,
            d.OrderId,
            d.DocumentType,
            d.DocumentNumber,
            d.FileUrl,
            d.FileName,
            d.FileSizeBytes,
            d.VerificationStatus,
            d.VerifiedByStaffId,
            d.VerifiedAt,
            d.RejectionReason,
            d.CreatedAt));
    }
}
