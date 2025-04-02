using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// variant of demolish visual that copies over materials and properties from a renderer(similar to <see cref="BuildingAddonRenderer"/>)<br/>
    /// can be used to put things like animated dissolves on the demolish visual
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual">https://citybuilder.softleitner.com/manual</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_demolish_visual_renderer.html")]
    public class DemolishVisualRenderer : DemolishVisual
    {
        [Tooltip("paths to the transforms that contains the renderers starting from Pivot, empty for all")]
        public string[] Targets;
        [Tooltip("materials that are added to the renderers while the addon is active")]
        public Renderer Renderer;
        [Tooltip("whether and how materials from the above renderer are put on the building renderers(may not be necessary if materials on target already support needed features like dissolve)")]
        public BuildingAddonRenderer.MaterialMode CopyMaterial;
        [Tooltip("whether the property block is copied from the renderer to the building(which is used in animations)")]
        public bool CopyMaterialBlock;
        [Tooltip("whether regular properties are copied from the renderer to the building")]
        public bool CopyProperties;

        private Renderer[] _renderers;

        private void Start()
        {
            _renderers = getRenderers();

            if (CopyMaterial == BuildingAddonRenderer.MaterialMode.Replace)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.sharedMaterials = Renderer.sharedMaterials;
                }
            }
            else if (CopyMaterial == BuildingAddonRenderer.MaterialMode.Add)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.sharedMaterials = renderer.sharedMaterials.Concat(Renderer.sharedMaterials).ToArray();
                }
            }
        }

        private void Update()
        {
            foreach (var renderer in _renderers)
            {
                if (CopyMaterialBlock)
                {
                    var block = new MaterialPropertyBlock();

                    for (var i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        Renderer.GetPropertyBlock(block);
                        renderer.SetPropertyBlock(block);
                    }
                }

                if (CopyProperties)
                {
                    for (var i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        renderer.sharedMaterials[i].CopyPropertiesFromMaterial(Renderer.sharedMaterials[i]);
                    }
                }
            }
        }

        private Renderer[] getRenderers()
        {
            if (Targets != null && Targets.Length > 0)
            {
                return Targets.Select(t => transform.GetChild(0).Find(t).GetComponent<Renderer>()).Where(r => r is ParticleSystemRenderer == false).ToArray();
            }
            else
            {
                return transform.GetChild(0).GetComponentsInChildren<Renderer>().Where(r => r is ParticleSystemRenderer == false).ToArray();
            }
        }
    }
}