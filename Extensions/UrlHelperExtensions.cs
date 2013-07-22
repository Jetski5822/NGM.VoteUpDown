using System.Web.Mvc;

namespace NGM.VoteUpDown.Extensions {
    public static class UrlHelperExtensions {

        // Default Route
        public static string ApplyVote(this UrlHelper urlHelper) {
            return urlHelper.Action("Apply", "Vote", new { area = Constants.LocalArea });
        }
    }
}