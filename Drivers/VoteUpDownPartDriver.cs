using System.Linq;
using Contrib.Voting.Models;
using Contrib.Voting.Services;
using NGM.VoteUpDown.Models;
using Orchard;
using Orchard.ContentManagement.Drivers;

namespace NGM.VoteUpDown.Drivers {
    public class VoteUpDownPartDriver : ContentPartDriver<VoteUpDownPart> {
        private readonly IOrchardServices _orchardServices;
        private readonly IVotingService _votingService;

        public VoteUpDownPartDriver(IOrchardServices orchardServices, IVotingService votingService) {
            _orchardServices = orchardServices;
            _votingService = votingService;
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
            part.ResultValue = (_votingService.GetResult(part.ContentItem.Id, "sum")
                ?? new ResultRecord()).Value;
                    
            // get the user's vote
            var currentUser = _orchardServices.WorkContext.CurrentUser;
            if (currentUser != null) {
                var userRating = _votingService.Get(vote => vote.Username == currentUser.UserName && vote.ContentItemRecord == part.ContentItem.Record).FirstOrDefault();
                part.UserRating = userRating != null ? userRating.Value : 0;
            }

            return part;
        }
    }
}