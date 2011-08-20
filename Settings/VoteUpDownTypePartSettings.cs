using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace NGM.VoteUpDown.Settings {
    public class VoteUpDownTypePartSettings {
        private bool? _showVoter;
        public bool ShowVoter {
            get {
                if (_showVoter == null)
                    _showVoter = true;
                return (bool)_showVoter;
            }
            set { _showVoter = value; }
        }

        private bool? _allowAnonymousRatings;
        public bool AllowAnonymousRatings {
            get {
                if (_allowAnonymousRatings == null)
                    _allowAnonymousRatings = false;
                return (bool)_allowAnonymousRatings;
            }
            set { _allowAnonymousRatings = value; }
        }
    }

    public class ContainerSettingsHooks : ContentDefinitionEditorEventsBase {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "VoteUpDownPart")
                yield break;

            var model = definition.Settings.GetModel<VoteUpDownTypePartSettings>();

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "VoteUpDownPart")
                yield break;

            var model = new VoteUpDownTypePartSettings();
            updateModel.TryUpdateModel(model, "VoteUpDownTypePartSettings", null, null);
            builder.WithSetting("VoteUpDownTypePartSettings.ShowVoter", model.ShowVoter.ToString());
            builder.WithSetting("VoteUpDownTypePartSettings.AllowAnonymousRatings", model.AllowAnonymousRatings.ToString());

            yield return DefinitionTemplate(model);
        }
    }
}