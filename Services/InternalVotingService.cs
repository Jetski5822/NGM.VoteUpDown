using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Contrib.Voting.Models;
using Contrib.Voting.Services;
using NGM.VoteUpDown.Extensions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc;
using Orchard.Security;

namespace NGM.VoteUpDown.Services {
    public interface IInternalVotingService : IDependency {
        double GetResult(ContentItem contentItem);
        VoteRecord GetUserVote(IUser user, ContentItem contentItem);
        VoteRecord GetAnonUserVote(ContentItem contentItem);
        void RegisterUserVote(IUser user, ContentItem contentItem, int rating);
        void RegisterAnonVote(ContentItem contentItem, int rating);
    }

    public class InternalVotingService : IInternalVotingService {
        private readonly IVotingService _votingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalVotingService(IVotingService votingService,
            IHttpContextAccessor httpContextAccessor) {
            _votingService = votingService;
            _httpContextAccessor = httpContextAccessor;
        }

        public double GetResult(ContentItem contentItem) {
            var result = _votingService.GetResult(contentItem.Id, "sum", Constants.Voting.RatingConstant);
            if (result == null)
                return 0D;
            return result.Value;
        }

        public VoteRecord GetUserVote(IUser user, ContentItem contentItem) {
            return _votingService.Get(vote =>
               vote.Username == user.UserName
            && vote.ContentItemRecord == contentItem.Record
            && vote.Dimension == Constants.Voting.RatingConstant).FirstOrDefault();
        }

        public VoteRecord GetAnonUserVote(ContentItem contentItem) {
            var currentContext = _httpContextAccessor.Current();
            var anonHostname = currentContext.Request.UserHostAddress;
            if (!string.IsNullOrWhiteSpace(currentContext.Request.Headers["X-Forwarded-For"]))
                anonHostname += "-" + currentContext.Request.Headers["X-Forwarded-For"];

            return _votingService.Get(vote => vote.Username == "Anonymous"
                    && vote.Hostname == anonHostname
                    && vote.ContentItemRecord == contentItem.Record
                    && vote.Dimension == Constants.Voting.RatingConstant).FirstOrDefault();
        }

        public void RegisterUserVote(IUser user, ContentItem contentItem, int rating) {
            var currentVote = GetUserVote(user, contentItem);

            if (currentVote != null && (currentVote.Value + rating == 0)) {
                _votingService.RemoveVote(currentVote);
            }
            else {
                if (currentVote != null)
                    _votingService.ChangeVote(currentVote, rating);
                else
                    _votingService.Vote(contentItem, user.UserName, _httpContextAccessor.Current().Request.UserHostAddress, rating, Constants.Voting.RatingConstant);
            }
        }

        public void RegisterAnonVote(ContentItem contentItem, int rating) {
            var currentContext = _httpContextAccessor.Current();
            var anonHostname = currentContext.Request.UserHostAddress;
            if (!string.IsNullOrWhiteSpace(currentContext.Request.Headers["X-Forwarded-For"]))
                anonHostname += "-" + currentContext.Request.Headers["X-Forwarded-For"];

            _votingService.Vote(contentItem, "Anonymous", anonHostname, rating, Constants.Voting.RatingConstant);
        }
    }
}