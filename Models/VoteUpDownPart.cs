using Orchard.ContentManagement;

namespace NGM.VoteUpDown.Models {
    public class VoteUpDownPart : ContentPart {
        public bool ShowVoter { get; set; }

        public bool AllowAnonymousRatings { get; set; }

        public double ResultValue { get; set; }

        public double UserRating { get; set; }
    }
}