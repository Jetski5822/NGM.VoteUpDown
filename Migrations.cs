using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Localization;

namespace NGM.VoteUpDown {
    public class Migrations : DataMigrationImpl {
        public Migrations() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public int Create() {
            ContentDefinitionManager.AlterPartDefinition("VoteUpDownPart", builder => builder
                .Attachable()
                .WithDescription(T("Allows a user to vote up or down on a content type").Text));

            return 2;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterPartDefinition("VoteUpDownPart", builder => builder
                .WithDescription(T("Allows a user to vote up or down on a content type").Text));

            return 2;
        }
    }
}