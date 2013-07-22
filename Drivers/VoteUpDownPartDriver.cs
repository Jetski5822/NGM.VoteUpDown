using Contrib.Voting.Models;
using NGM.VoteUpDown.Extensions;
using NGM.VoteUpDown.Models;
using NGM.VoteUpDown.Services;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Security;

namespace NGM.VoteUpDown.Drivers {
    public class VoteUpDownPartDriver : ContentPartDriver<VoteUpDownPart> {
        private readonly IAuthenticationService _authenticationService;
        private readonly IInternalVotingService _internalVotingService;

        public VoteUpDownPartDriver(IAuthenticationService authenticationService, IInternalVotingService internalVotingService) {
            _authenticationService = authenticationService;
            _internalVotingService = internalVotingService;
        }

        protected override DriverResult Display(VoteUpDownPart part, string displayType, dynamic shapeHelper) {
            if (!part.ShowVoter)
                return null;

            var displayPart = BuildVoteUpDown(part);

            return Combined(
                ContentShape(
                    "Parts_VoteUpDown",
                        () => shapeHelper.Parts_VoteUpDown(displayPart)),
                ContentShape(
                    "Parts_VoteUpDown_Summary",
                        () => shapeHelper.Parts_VoteUpDown_Summary(displayPart)),
                ContentShape(
                    "Parts_VoteUpDown_UserSummary",
                        () => shapeHelper.Parts_VoteUpDown_UserSummary(displayPart)),
                ContentShape(
                    "Parts_VoteUpDown_SummaryAdmin",
                        () => shapeHelper.Parts_VoteUpDown_SummaryAdmin(displayPart))
                );
        }

        private VoteUpDownPart BuildVoteUpDown(VoteUpDownPart part) {
            part.ResultValue =
                _internalVotingService.GetResult(part.ContentItem);
                    
            // get the user's vote
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser != null) {
                var rating = _internalVotingService.GetUserVote(currentUser, part.ContentItem);
                part.UserRating = rating != null ? rating.Value : 0;
            }

            return part;
        }
    }
}