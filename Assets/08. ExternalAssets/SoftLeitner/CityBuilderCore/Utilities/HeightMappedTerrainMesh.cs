using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// extended version of <see cref="HeightMappedTerrainRenderer"/> that can be used when height mapped is used in combination with a regular mesh renderer instead of a tilemap<br/>
    /// used in combination with <see cref="MeshOverlayGenerator"/> in DebutTerrain of the Three demo in the Overlays_MeshHM overlay variant
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual">https://citybuilder.softleitner.com/manual</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_height_mapped_terrain_mesh.html")]
    [ExecuteAlways]
    public class HeightMappedTerrainMesh : HeightMappedTerrainRenderer
    {
        [Header("Mesh Spezific")]
        [Tooltip("optional, when set the texture is assigned to this material property")]
        public string TextureName = "_MainTex";
        [Tooltip("texture that is assigned to the above property when set")]
        public Texture Texture;
        [Tooltip("prevents culling of the renderer by expanding its bounds to max")]
        public bool PreventCulling = true;

        void OnEnable()
        {
            Assign();
        }

        public override void Assign()
        {
            base.Assign();

            if (PreventCulling)
                Renderer.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
        }

        protected override void assignProperties(MaterialPropertyBlock propertyBlock)
        {
            base.assignProperties(propertyBlock);

            if (!string.IsNullOrWhiteSpace(TextureName))
                propertyBlock.SetTexture(TextureName, Texture);
        }
    }
}
