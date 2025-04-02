using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// copies over materials and properties from a renderer to the building while active<br/>
    /// can be used to put things like animated dissolves on a building temporarily<br/>
    /// (see BuildingPlayground3D for an example in the BigHouseTool_Appear tool)
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/buildings">https://citybuilder.softleitner.com/manual/buildings</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_building_addon_renderer.html")]
    public class BuildingAddonRenderer : BuildingAddon
    {
        public enum MaterialMode { Replace, Add, None }

        [Tooltip("paths to the transforms that contains the renderers starting from Pivot, empty for all")]
        public string[] Targets;
        [Tooltip("materials that are added to the renderers while the addon is active")]
        public Renderer Renderer;
        [Tooltip("whether and how materials from the above renderer are put on the building renderers(may not be necessary if materials on target already support needed features like dissolve)")]
        public MaterialMode CopyMaterial;
        [Tooltip("whether the property block is copied from the renderer to the building(which is used in animations)")]
        public bool CopyMaterialBlock;
        [Tooltip("whether regular properties are copied from the renderer to the building")]
        public bool CopyProperties;

        private Renderer[] _renderers;
        private List<Material[]> _originalMaterials;

        public override void InitializeAddon()
        {
            base.InitializeAddon();

            _renderers = getRenderers();

            if (CopyMaterial == MaterialMode.Replace)
            {
                _originalMaterials = new List<Material[]>();
                foreach (var renderer in _renderers)
                {
                    _originalMaterials.Add(renderer.sharedMaterials);
                    renderer.sharedMaterials = Renderer.sharedMaterials;
                }
            }
            else if (CopyMaterial == MaterialMode.Add)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.sharedMaterials = renderer.sharedMaterials.Concat(Renderer.sharedMaterials).ToArray();
                }
            }
        }

        public override void Update()
        {
            base.Update();

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

        public override void TerminateAddon()
        {
            base.TerminateAddon();

            if (CopyMaterial == MaterialMode.Replace)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (!_renderers[i])
                        continue;
                    _renderers[i].sharedMaterials = _originalMaterials[i];
                }
            }
            else if (CopyMaterial == MaterialMode.Add)
            {
                foreach (var renderer in _renderers)
                {
                    if (!renderer)
                        continue;
                    renderer.sharedMaterials = renderer.sharedMaterials.SkipLast(Renderer.sharedMaterials.Length).ToArray();
                }
            }
        }

        private Renderer[] getRenderers()
        {
            if (Targets != null && Targets.Length > 0)
            {
                return Targets.Select(t => Building.Pivot.Find(t).GetComponent<Renderer>()).Where(r => r is ParticleSystemRenderer == false).ToArray();
            }
            else
            {
                return Building.Pivot.GetComponentsInChildren<Renderer>().Where(r => r is ParticleSystemRenderer == false).ToArray();
            }
        }
    }
}