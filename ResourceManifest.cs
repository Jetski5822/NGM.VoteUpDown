using Orchard.UI.Resources;

namespace NGM.VoteUpDown {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("NGM.VoteUpDown").SetUrl("NGM.VoteUpDown.css");
            manifest.DefineScript("NGM.VoteUpDown").SetUrl("NGM.VoteUpDown.js").SetVersion("1.0").SetDependencies("jQuery", "ShapesBase");
        }
    }
}
