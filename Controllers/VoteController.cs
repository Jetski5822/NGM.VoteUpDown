using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Contrib.Voting.Services;
using NGM.VoteUpDown.Handlers;
using NGM.VoteUpDown.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Extensions;

namespace NGM.VoteUpDown.Controllers {
    public class VoteController  : Controller {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IVotingService _votingService;

        public VoteController(IOrchardServices orchardServices, IContentManager contentManager, IVotingService votingService) {
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _votingService = votingService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [HttpPost, ActionName("Apply")]
        [FormValueRequired("submit.UpVote")]
        public ActionResult Apply_UpVote(int contentId, string returnUrl) { 
            return Apply(contentId, true, returnUrl); 
        }

        [HttpPost, ActionName("Apply")]
        [FormValueRequired("submit.DownVote")]
        public ActionResult Apply_DownVote(int contentId, string returnUrl) {
            return Apply(contentId, false, returnUrl);
        }

        private ActionResult Apply(int contentId, bool voteUp, string returnUrl) {
            var content = _contentManager.Get(contentId);
            if (content == null || !content.Has<VoteUpDownPart>() || !content.As<VoteUpDownPart>().ShowVoter)
                return this.RedirectLocal(returnUrl, "~/");

            var currentUser = _orchardServices.WorkContext.CurrentUser;
            if ((currentUser == null && !content.As<VoteUpDownPart>().AllowAnonymousRatings))
                return this.RedirectLocal(returnUrl, "~/");

            int rating = voteUp ? 1 : -1;

            if (currentUser != null) {
                var currentVote = _votingService.Get(vote => vote.Username == currentUser.UserName && vote.ContentItemRecord == content.Record && vote.Dimension == Constants.Dimension).FirstOrDefault();

                if (currentVote != null && (currentVote.Value + rating == 0)) {
                    _votingService.RemoveVote(currentVote);
                }
                else {
                    if (currentVote != null)
                        _votingService.ChangeVote(currentVote, rating);
                    else
                        _votingService.Vote(content, currentUser.UserName, HttpContext.Request.UserHostAddress, rating, Constants.Dimension);
                }
            }
            else {
                var anonHostname = HttpContext.Request.UserHostAddress;
                if (!string.IsNullOrWhiteSpace(HttpContext.Request.Headers["X-Forwarded-For"]))
                    anonHostname += "-" + HttpContext.Request.Headers["X-Forwarded-For"];

                var currentVote = _votingService.Get(vote => vote.Username == "Anonymous" && vote.Hostname == anonHostname && vote.ContentItemRecord == content.Record && vote.Dimension == Constants.Dimension).FirstOrDefault();
                if (rating > 0 && currentVote == null)
                    _votingService.Vote(content, "Anonymous", anonHostname, rating, Constants.Dimension);
            }

            return this.RedirectLocal(returnUrl, "~/");
        }
    }

    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute {
        private readonly string _submitButtonName;

        public FormValueRequiredAttribute(string submitButtonName) {
            _submitButtonName = submitButtonName;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
            var value = controllerContext.HttpContext.Request.Form[_submitButtonName];
            return !string.IsNullOrEmpty(value);
        }
    }
}