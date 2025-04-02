using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// view that enables named renderers while active
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/views">https://citybuilder.softleitner.com/manual/views</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_view_renderer.html")]
    [CreateAssetMenu(menuName = "CityBuilder/Views/" + nameof(ViewRenderer))]
    public class ViewRenderer : View
    {
        [Tooltip("name of the gameobject that will get have all renderers on it enabled while the view is active")]
        public string ObjectName;

        public override void Activate() => GameObject.Find(ObjectName).GetComponents<Renderer>().ForEach(r => r.enabled = true);
        public override void Deactivate() => GameObject.Find(ObjectName).GetComponents<Renderer>().ForEach(r => r.enabled = false);
    }
}