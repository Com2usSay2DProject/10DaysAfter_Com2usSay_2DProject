using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// addon that transfers its scale, rotation and position to the attached building<br/>
    /// this can be used to attach animations to any building by animating the addon<br/><br/>
    /// in THREE this is used to make buildings pop up with an animation when they are built<br/>
    /// to make this happen a BuildingAddonTransform with an animation is assigned to <see cref="DefaultBuildingManager.AddingAddon"/>
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/buildings">https://citybuilder.softleitner.com/manual/buildings</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_building_addon_transformer.html")]
    public class BuildingAddonTransformer : BuildingAddon
    {
        [Tooltip("resets scale, rotation and position of the addon on initialize")]
        public bool ResetTransform = true;
        [Tooltip("sets scale to 0 on initialize so the building is hidden until the first update, good for spawners that start hidden")]
        public bool InitializeHidden;

        private Vector3 _pivotScale;
        private Quaternion _pivotRotation;
        private Vector3 _pivotPosition;

        private Vector3 _addonScale;
        private Quaternion _addonRotation;
        private Vector3 _addonPosition;

        public override void InitializeAddon()
        {
            base.InitializeAddon();

            if (ResetTransform)
            {
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
                transform.localPosition = Vector3.zero;

                _pivotScale = Building.Pivot.localScale;
                _pivotRotation = Building.Pivot.localRotation;
                _pivotPosition = Building.Pivot.localPosition;
            }
            else
            {
                _addonScale = transform.localScale;
                _addonRotation = transform.localRotation;
                _addonPosition = transform.localPosition;

                _pivotScale = Building.Pivot.localScale;
                _pivotRotation = Building.Pivot.localRotation;
                _pivotPosition = Building.Pivot.localPosition;
            }

            if (InitializeHidden)
            {
                Building.Pivot.localScale = Vector3.zero;
            }
        }

        public override void TerminateAddon()
        {
            base.TerminateAddon();

            Building.Pivot.localScale = _pivotScale;
            Building.Pivot.localRotation = _pivotRotation;
            Building.Pivot.localPosition = _pivotPosition;
        }

        public override void Update()
        {
            base.Update();

            if (_isTerminated)
                return;

            if (ResetTransform)
            {
                Building.Pivot.localScale = Vector3.Scale(_pivotScale, transform.localScale);
                Building.Pivot.localRotation = _pivotRotation * transform.localRotation;
                Building.Pivot.localPosition = _pivotPosition + transform.localPosition;
            }
            else
            {
                var deltaScale = new Vector3(transform.localScale.x / _addonScale.x, transform.localScale.y / _addonScale.y, transform.localScale.z / _addonScale.z);
                var deltaRotation = transform.localRotation * Quaternion.Inverse(_addonRotation);
                var deltaPosition = transform.localPosition - _addonPosition;

                Building.Pivot.localScale = Vector3.Scale(_pivotScale, deltaScale);
                Building.Pivot.localRotation = _pivotRotation * deltaRotation;
                Building.Pivot.localPosition = _pivotPosition + deltaPosition;

            }
        }
    }
}