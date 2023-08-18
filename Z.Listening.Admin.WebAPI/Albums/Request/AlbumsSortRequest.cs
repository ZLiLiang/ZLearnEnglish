using FluentValidation;
using Z.Commons.Validators;

namespace Z.Listening.Admin.WebAPI.Albums.Request
{
    public class AlbumsSortRequest
    {
        public Guid[] SortedAlbumIds { get; set; }
    }

    public class AlbumsSortRequestValidator : AbstractValidator<AlbumsSortRequest>
    {
        public AlbumsSortRequestValidator()
        {
            RuleFor(r => r.SortedAlbumIds)
                .NotNull()
                .NotEmpty()
                .NotContains(Guid.Empty)
                .NotDuplicated();
        }
    }
}
