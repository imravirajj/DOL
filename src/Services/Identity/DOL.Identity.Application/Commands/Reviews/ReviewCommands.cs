using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Reviews;

// ── Create Review ───────────────────────────────────────────
public record CreateReviewCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    int Rating,
    string Title,
    string ReviewText,
    Guid? OrderId = null) : IRequest<Result<Guid>>;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.ReviewText).NotEmpty();
    }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateReviewCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        bool isVerified = false;
        if (request.OrderId.HasValue)
        {
            isVerified = await _context.VehicleOrders
                .AnyAsync(o => o.Id == request.OrderId.Value && o.BuyerId == request.BuyerId, cancellationToken);
        }

        var review = new DealershipReview(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            request.Rating,
            request.Title,
            request.ReviewText,
            request.OrderId,
            isVerified);

        _context.DealershipReviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(review.Id);
    }
}

// ── Respond to Review ───────────────────────────────────────
public record RespondToReviewCommand(
    Guid ReviewId,
    string Response) : IRequest<Result<bool>>;

public class RespondToReviewCommandHandler : IRequestHandler<RespondToReviewCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public RespondToReviewCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RespondToReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _context.DealershipReviews.FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);
        if (review == null) return Result<bool>.Failure("Review not found.");

        review.Respond(request.Response);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Get Reviews Query ───────────────────────────────────────
public record GetDealershipReviewsQuery(Guid? BranchId = null) : IRequest<Result<List<DealershipReviewDto>>>;

public class GetDealershipReviewsQueryHandler : IRequestHandler<GetDealershipReviewsQuery, Result<List<DealershipReviewDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetDealershipReviewsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<DealershipReviewDto>>> Handle(GetDealershipReviewsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DealershipReviews.AsNoTracking();
        if (request.BranchId.HasValue)
        {
            query = query.Where(r => r.BranchId == request.BranchId.Value);
        }

        var list = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new DealershipReviewDto(
                r.Id,
                r.CompanyId,
                r.BranchId,
                r.BuyerId,
                r.OrderId,
                r.Rating,
                r.Title,
                r.ReviewText,
                r.IsVerifiedBuyer,
                r.DealerResponse,
                r.RespondedAt,
                r.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<DealershipReviewDto>>.Success(list);
    }
}

public record GetReviewByIdQuery(Guid Id) : IRequest<Result<DealershipReviewDto>>;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Result<DealershipReviewDto>>
{
    private readonly IIdentityDbContext _context;

    public GetReviewByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DealershipReviewDto>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _context.DealershipReviews.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (r == null) return Result<DealershipReviewDto>.Failure("Review not found.");

        return Result<DealershipReviewDto>.Success(new DealershipReviewDto(
            r.Id,
            r.CompanyId,
            r.BranchId,
            r.BuyerId,
            r.OrderId,
            r.Rating,
            r.Title,
            r.ReviewText,
            r.IsVerifiedBuyer,
            r.DealerResponse,
            r.RespondedAt,
            r.CreatedAt));
    }
}
