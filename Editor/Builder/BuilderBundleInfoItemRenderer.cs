#if UNITY_EDITOR

using Build1.UnityEGUI;
using Build1.UnityEGUI.PropertyList;
using Build1.UnityEGUI.RenderModes;
using Build1.UnityEGUI.Results;

namespace Build1.UnityAssetBundlesTool.Editor.Builder
{
    public sealed class BuilderBundleInfoItemRenderer : PropertyListItemRenderer<BuilderBundleInfo>
    {
        public override void OnEGUI()
        {
            if (Item.ExcludeFromBuilderScope)
                return;
            
            EGUI.Panel(10, () =>
            {
                EGUI.Enabled(false, () => { EGUI.Property(Item, Item.Name, nameof(Item.Name)); });

                EGUI.Horizontally(() =>
                {
                    EGUI.Property(Item, Item.Version, nameof(Item.Version), NumericRenderMode.Field, EGUI.ButtonHeight04);
                    EGUI.Button("+", EGUI.Width(100)).OnClick(Item.IncrementVersion);
                    EGUI.Button("-", EGUI.Width(100)).OnClick(Item.DecrementVersion);
                });

                var include = Item.IncludeInBuildSequence;

                EGUI.Horizontally(() =>
                {
                    EGUI.Property(Item, Item.IncludeInBuildSequence, nameof(Item.IncludeInBuildSequence), BooleanRenderMode.Dropdown, EGUI.ButtonHeight04);
                    EGUI.Button("Exclude", EGUI.Width(203)).OnClick(() =>
                    {
                        EGUI.Alert("Asset Bundles Builder", $"Are you sure you want to exclude \"{Item.Name}\" bundle from the scope of the Builder?\n\nThis can be reverted via manual config editing and Builder windows reopening.", (result) =>
                        {
                            if (result != AlertResult.Confirm) 
                                return;
                            
                            EGUI.PropertySet(Item, Item.IncludeInBuildSequence, nameof(Item.IncludeInBuildSequence), false);
                            Item.ExcludeFromBuilder();
                        });
                    });
                });

                if (Item.IncludeInBuildSequence != include)
                    Item.SetDirty();
            });
        }
    }
}

#endif