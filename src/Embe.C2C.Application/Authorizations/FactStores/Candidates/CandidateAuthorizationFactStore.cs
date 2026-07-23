using System.Data.Common;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Candidates.Facts;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Domain.Aggregates.Candidates;

namespace Embe.C2C.Application.Authorizations.FactStores.Candidates;

public class CandidateAuthorizationFactStore
(
    IAuthenticatedUserService authenticatedUserService,
    CandidateAuthorizationFactGenerator candidateFactGenerator
) : AuthorizationFactStore(authenticatedUserService)
{
    private readonly CandidateAuthorizationFactGenerator _candidateFactGenerator = candidateFactGenerator;

    public async Task<IsOwner> GetIsOwnerAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsOwner>(candidateId);
        if (fact != null)
        {
            return fact;
        }
        await LoadFactsAsync(candidateId, cancellationToken);
        return GetFact<IsOwner>(candidateId) ?? throw new InvalidOperationException("No 'IsOwner' fact present after loading all facts");
    }

    public IsOwner GetIsOwner(Candidate candidate)
    {
        return GetFact<IsOwner>(candidate.Id) ?? SetFact(new IsOwner(candidate.Id, candidate.UserId == CurrentUserId));
    }

    public IsCandidate GetIsCandidate(Candidate candidate)
    {
        return GetFact<IsCandidate>(candidate.Id) ?? SetFact(new IsCandidate(candidate.Id, candidate.CandidateUserId == CurrentUserId));
    }

    public async Task<IsCandidate> GetIsCandidateAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsCandidate>(candidateId);
        if (fact != null)
        {
            return fact;
        }
        await LoadFactsAsync(candidateId, cancellationToken);
        return GetFact<IsCandidate>(candidateId) ?? throw new InvalidOperationException("No 'IsCandidate' fact present after loading all facts");
    }

    private async Task LoadFactsAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        var facts = await _candidateFactGenerator.GetAuthorizationFactsAsync(candidateId, cancellationToken);
        foreach (var fact in facts)
        {
            SetFact(fact);
        }
    }

    internal IsPositivelyJudgedCandidate GetIsPositivelyJudgedCandidate(Candidate candidate)
    {
        return GetFact<IsPositivelyJudgedCandidate>(candidate.Id) ??
               SetFact(new IsPositivelyJudgedCandidate(candidate.Id, candidate.CandidateUserId == CurrentUserId && candidate.Judgement == true));
    }

    internal async Task<IsPositivelyJudgedCandidate> GetIsPositivelyJudgedCandidateAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsPositivelyJudgedCandidate>(candidateId);
        if (fact != null)
            return fact;
        await LoadFactsAsync(candidateId, cancellationToken);
        return GetFact<IsPositivelyJudgedCandidate>(candidateId) ?? throw new InvalidOperationException("No 'IsPositivelyJudgedCandidate' fact present after loading all facts");
    }
}