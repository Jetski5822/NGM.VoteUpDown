using Orchard.UI.Resources;

namespace NGM.VoteUpDown {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("NGM.VoteUpDown").SetUrl("NGM.VoteUpDown.css");
        }
    }
}
