using NGM.VoteUpDown.Models;
using NGM.VoteUpDown.Settings;
using Orchard.ContentManagement.Handlers;

namespace NGM.VoteUpDown.Handlers {
    public class VoteUpDownPartHandler : ContentHandler {
        public VoteUpDownPartHandler() {
            OnInitializing<VoteUpDownPart>((context, part) => {
                                               part.ShowVoter = part.Settings.GetModel<VoteUpDownTypePartSettings>().ShowVoter;
                                               part.AllowAnonymousRatings = part.Settings.GetModel<VoteUpDownTypePartSettings>().AllowAnonymousRatings;
                                           });
        }
    }
}