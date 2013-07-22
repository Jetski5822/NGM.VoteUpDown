using System.Reflection;
using System.Web.Mvc;
using NGM.VoteUpDown.Models;
using NGM.VoteUpDown.Services;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Security;

namespace NGM.VoteUpDown.Controllers {
    public class VoteController  : Controller {
        private readonly IContentManager _contentManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IInternalVotingService _internalVotingService;

        public VoteController(
            IContentManager contentManager, 
            IAuthenticationService authenticationService, 
            IInternalVotingService internalVotingService) {
            _contentManager = contentManager;
            _authenticationService = authenticationService;
            _internalVotingService = internalVotingService;
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

            var currentUser = _authenticationService.GetAuthenticatedUser();
            if ((currentUser == null && !content.As<VoteUpDownPart>().AllowAnonymousRatings))
                return this.RedirectLocal(returnUrl, "~/");

            int rating = voteUp ? 1 : -1;

            if (currentUser != null) {
                _internalVotingService.RegisterUserVote(currentUser, content, rating);
            }
            else {
                var currentVote = _internalVotingService.GetAnonUserVote(content);

                if (rating > 0 && currentVote == null)
                    _internalVotingService.RegisterAnonVote(content, rating);
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