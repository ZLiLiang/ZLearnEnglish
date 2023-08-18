using FluentValidation;
using Z.Commons.Validators;

namespace Z.Listening.Admin.WebAPI.Episodes.Request
{
    public class EpisodesSortRequest
    {
        public Guid[] SortedEpisodeIds { get; set; }
    }

    public class EpisodesSortRequestValidator : AbstractValidator<EpisodesSortRequest>
    {
        public EpisodesSortRequestValidator()
        {
            RuleFor(r => r.SortedEpisodeIds)
                .NotNull()
                .NotEmpty()
                .NotContains(Guid.Empty)
                .NotDuplicated();
        }
    }
}
